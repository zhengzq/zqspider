using System;
using System.Collections.Generic;
using ZqSpider.Core.Spiders;
using ZqSpider.Logging;

namespace ZqSpider.Core
{
    public class SpiderLoader
    {
        private readonly Dictionary<string, ISpider> _spiderDic;

        public SpiderLoader(ISpider[] spiders, ILogger logger)
        {
            if (spiders == null)
                throw new ArgumentNullException(nameof(spiders));

            this._spiderDic = new Dictionary<string, ISpider>();

            foreach (var spider in spiders)
            {
                try
                {
                    this._spiderDic.Add(spider.Name, spider);
                }
                catch (ArgumentException)
                {
                    logger.ErrorFormat("there are several spiders with the same name :{0}", spider.Name);
                    throw;
                }
            }

        }

        public ISpider GetBy(string name)
        {
            return _spiderDic[name];
        }

    }
}
