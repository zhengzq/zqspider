using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZqSpider.Core;
using ZqSpider.Core.Spiders;

namespace Example.Simple
{
    public class SimpleSpider : ISpider
    {
        public string Name
        {
            get { return "Simple"; }
        }

        public bool NonFirst { get; set; }

        public SpiderResponse Extract(Response response)
        {
            var item = new Item();

            //var reg = new Regex(@"(?is)<a(?:(?!href=).)*href=(['""]?)(?<url>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");
            //var mc = reg.Matches(response.Html);
            var mc = Regex.Matches(response.Html, @"(?is)<a(?:(?!href=).)*href=(['""]?)(?<url>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");
            var rs = new List<Request>();
            foreach (Match m in mc)
            {
                var url = m.Groups["url"].Value;
                if (string.IsNullOrEmpty(url))
                    continue;

                var m1 = Regex.Match(url, @"((ht|f)tps?):\/\/[\w\-]+(\.[\w\-]+)+([\w\-\.,@?^=%&:\/~\+#]*[\w\-\@?^=%&\/~\+#])?");
                if (string.IsNullOrEmpty(m1.Value) || !m1.Success)
                    continue;

                rs.Add(new Request()
                {
                    Url = m1.Value,
                    AllowAutoRedirect = true
                });
            }
            item.Data.Add("url", response.Request.Url);
            return SpiderResponse.Create(item, rs.ToArray());
        }

        public Request[] Requests
        {
            get
            {
                return new Request[]
                {
                    new Request()
                    {
                        Url = "http://www.ttmeiju.com/",
                        AllowAutoRedirect = true
                    }
                };
            }
        }
    }
}
