using System;
using System.Collections.Generic;
using System.Text;

namespace ZqSpider.Core.Spiders
{
    public class SpiderResponse
    {
        public SpiderResponse() { }
        public SpiderResponse(Item item, Request[] requests)
        {
            this.Item = item;
            this.NewRequests = requests;
        }

        public Item Item { get; set; }
        public Request[] NewRequests { get; set; }

        public static SpiderResponse Create(Item item, Request[] requests)
        {
            return new SpiderResponse(item, requests);
        }
    }
}
