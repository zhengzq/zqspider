using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZqSpider.Core.Spiders;
using ZqSpider.Logging;

namespace ZqSpider.Core.Downloaders
{
    public class Downloader : IDownloader
    {
        private readonly ConcurrentBag<Request> _actives;
        private readonly int _totalConcurrency = 0;
        private readonly Dictionary<string, DownloadHandler> _handlers = new Dictionary<string, DownloadHandler>();
        private readonly Setting _setting;
        private readonly ILogger _logger;
        private readonly DownloaderMiddleware[] _downloaderMiddlewares;

        public Downloader(Dictionary<string, DownloadHandler> handlers, Setting setting, ILoggerFactory factory, DownloaderMiddleware[] downloaderMiddlewares)
        {
            this._handlers = handlers;
            this._actives = new ConcurrentBag<Request>();
            this._setting = setting;
            this._totalConcurrency = setting.TotalConcurrency;
            this._logger = factory.Create("downloader");
            this._downloaderMiddlewares = downloaderMiddlewares;
        }

        public bool Overload
        {
            get { return this._actives.Count >= _totalConcurrency; }
        }

        public bool Activing
        {
            get { return this._actives.Count > 0; }
        }

        public int Count { get => this._actives.Count; }

        public async Task<Response> AsyncFetch(Request request, ISpider spider)
        {
            try
            {
                this._actives.Add(request);
                var handler = GetDownloadHandler(request);
                if (handler == null)
                    return await Task.FromResult<Response>(null);
                request = await ProcessRequest(request, spider);
                var response = new Response() { Request = request };
                var result = await handler.Download(request, response, spider);
                return await ProcessResponse(request, response, spider);
            }
            catch (Exception ex)
            {
                throw new DownLoaderException(ex.Message + ("\n URL:" + request.Url), ex);
            }
            finally
            {
                this._actives.TryTake(out request);
            }
        }

        private async Task<Response> ProcessResponse(Request request, Response response, ISpider spider)
        {
            if (_downloaderMiddlewares != null)
                foreach (var downloaderMiddleware in this._downloaderMiddlewares)
                    response = await downloaderMiddleware.ProcessResponse(request, response, spider);
            return response;
        }

        private async Task<Request> ProcessRequest(Request request, ISpider spider)
        {
            if (_downloaderMiddlewares != null)
                foreach (var downloaderMiddleware in this._downloaderMiddlewares)
                    request = await downloaderMiddleware.ProcessRequest(request, spider);
            return request;
        }

        private DownloadHandler GetDownloadHandler(Request request)
        {
            try
            {
                var uri = new Uri(request.Url, UriKind.RelativeOrAbsolute);
                var handler = default(DownloadHandler);
                if (_handlers.TryGetValue(uri.Scheme, out handler))
                    return handler;
                throw new InvalidOperationException(string.Format("Unsupported URL scheme {0}", uri.Scheme));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
