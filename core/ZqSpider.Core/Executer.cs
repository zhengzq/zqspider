using System;
using System.Collections.Generic;
using ZqSpider.Configurations;
using ZqSpider.Core;
using ZqSpider.Core.Downloaders;
using ZqSpider.Core.Schedulers;
using ZqSpider.Core.Spiders;
using ZqSpider.Logging;

namespace ZqSpider
{
    public class Executer
    {
        private readonly Dictionary<string, Tuple<ISpider, Engine>> _dic;
        private readonly Configuration _configuration;
        private readonly ComponentManager _manager;

        public Executer(Configuration configuration = null)
        {
            this._configuration = configuration ?? Configuration.Instance.RegisterComponents();
            this._manager = this._configuration.ComponentManager;
            this._dic = new Dictionary<string, Tuple<ISpider, Engine>>();
        }

        public Executer AddSpider(ISpider spider, Setting setting = null)
        {
            setting = setting ?? new Setting();
            var scheduler = _manager.Resolve<IScheduler, Setting>(setting.SchedulerName, setting);
            var downloader = LoadDonloader(setting);
            var loggerFactory = _manager.Resolve<ILoggerFactory>();
            var itemPiplelines = LoadItemPipelines(setting);
            var engine = new Engine(spider, scheduler, downloader, loggerFactory, itemPiplelines);
            this._dic.Add(spider.Name, Tuple.Create(spider, engine));
            return this;
        }

        public void Start()
        {
            foreach (var item in this._dic)
                item.Value.Item2.Run();
        }

        public void Start(string spiderName)
        {
            if (this._dic.TryGetValue(spiderName, out Tuple<ISpider, Engine> item))
            {
                item.Item2.Run();
            }
            else
            {
                Console.WriteLine(string.Format("spider : {0} not exist!", spiderName));
            }
        }

        private IDownloader LoadDonloader(Setting setting)
        {
            var downloaHandlers = _manager.Resolve<Dictionary<string, DownloadHandler>>(setting.DownloadHandlersName);
            var downloaderMiddlewares = default(List<DownloaderMiddleware>);
            if (setting.DownloaderMiddlewareNames != null)
            {
                downloaderMiddlewares = new List<DownloaderMiddleware>();
                foreach (var item in setting.DownloaderMiddlewareNames)
                {
                    var downloaderMiddleware = _manager.Resolve<DownloaderMiddleware>(item);
                    if (downloaderMiddleware != null)
                        downloaderMiddlewares.Add(downloaderMiddleware);
                }
            }
            return _manager.Resolve<IDownloader, Dictionary<string, DownloadHandler>, Setting, DownloaderMiddleware[]>(setting.DownloaderName, downloaHandlers, setting, downloaderMiddlewares.ToArray());
        }

        private ItemPipeline[] LoadItemPipelines(Setting setting)
        {
            var itemPiplelines = default(List<ItemPipeline>);
            if (setting.PiplelineNames != null)
            {
                itemPiplelines = new List<ItemPipeline>();
                foreach (var item in setting.PiplelineNames)
                {
                    var itemPipeline = _manager.Resolve<ItemPipeline>(item);
                    if (itemPipeline != null)
                        itemPiplelines.Add(itemPipeline);
                }
            }
            return itemPiplelines.ToArray();
        }
    }
}
