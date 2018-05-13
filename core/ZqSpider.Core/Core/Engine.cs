using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZqSpider.Core.Downloaders;
using ZqSpider.Core.Queues;
using ZqSpider.Core.Schedulers;
using ZqSpider.Core.Spiders;
using ZqSpider.Logging;

namespace ZqSpider.Core
{
    public class Engine
    {
        private readonly IScheduler _scheduler;
        private readonly IDownloader _downloader;
        private readonly IRequestQueue _startRequests;
        private readonly ISpider _spider;
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly ConcurrentBag<Request> _actives;
        private readonly ItemPipeline[] _piplelines;
        private readonly ConcurrentBag<Request> _uncompleted;

        public Engine(ISpider spider, IScheduler scheduler, IDownloader downloader, ILoggerFactory factory, ItemPipeline[] piplelines = null)
        {
            this._spider = spider;
            this._scheduler = scheduler;
            this._downloader = downloader;
            this._logger = factory.Create("engine");
            this._startRequests = new MemoryQueue();
            this.Runing = false;
            this._actives = new ConcurrentBag<Request>();
            this._piplelines = piplelines;
            this._uncompleted = new ConcurrentBag<Request>();
        }

        public bool Runing { get; set; }

        public void Run()
        {
            this.Runing = true;
            _logger.InfoFormat("spider {0} start run", _spider.Name);
            foreach (var request in _spider.Requests)
                this._startRequests.Enqueue(request);

            this.StartTimer();
        }

        public void Stop()
        {
            this.Runing = false;
            if (this._timer != null)
                this._timer.Dispose();
            _logger.InfoFormat("spider {0} stop", _spider.Name);
        }

        private async Task NextRequest()
        {
            if (!this.Runing)
                return;

            while (!NeedWait)
            {
                var result = NextRequestFromScheduler();
                if (result == null)
                    continue;

                await this.NextRequest();
            }

            while (_startRequests.Count > 0)
            {
                var r = _startRequests.Dequeue();
                this._scheduler.Push(r);
            }

            if (this.SpiderIsIdle)
            {
                this.Stop();
                Console.WriteLine("done");
            }
        }

        private async Task<Tuple<Request, Response, Item>> NextRequestFromScheduler()
        {
            var request = this._scheduler.Pop();
            if (request == null)
                return null;

            try
            {
                this._actives.Add(request);
                var response = await this._downloader.AsyncFetch(request, _spider);
                if (response == null)
                    return null;

                var spiderResponse = _spider.Extract(response);
                var tasks = new List<Task>();
                if (spiderResponse.NewRequests != null)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var newRequest in spiderResponse.NewRequests)
                        {
                            newRequest.Deep = request.Deep + 1;
                            this._scheduler.Push(newRequest);
                        }
                    }));
                }

                if (this._piplelines != null && this._piplelines.Length > 0 && spiderResponse.Item != null)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        foreach (var pipleline in this._piplelines)
                            await pipleline.Process(spiderResponse.Item, this._spider);
                    }));
                }

                if (tasks.Count > 0)
                    await Task.WhenAll(tasks);

                return Tuple.Create(request, response, spiderResponse.Item);
            }
            catch (Exception ex)
            {
                _logger.Error(new EngineException(ex.Message + ("\n URL:" + request.Url), ex));
                this._uncompleted.Add(request);
                return null;
            }
            finally
            {
                this._actives.TryTake(out request);
            }
        }

        public bool SpiderIsIdle
        {
            get
            {
                if (this._actives.Count > 0)
                    return false;
                if (this._downloader.Activing)
                    return false;
                if (this._scheduler.HasPendingRequests)
                    return false;
                if (this._startRequests.Count > 0)
                    return false;
                return true;
            }
        }

        private bool NeedWait
        {
            get
            {
                if (this._scheduler.HasPendingRequests && !this._downloader.Overload)
                    return false;
                return true;
            }
        }

        private void StartTimer()
        {
            this._timer = new Timer(async (obj) =>
            {
                var s = obj as ISpider;
                await this.NextRequest();
            }, _spider, 0, 500);
        }
    }
}
