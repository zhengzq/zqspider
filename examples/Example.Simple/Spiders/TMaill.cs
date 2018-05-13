using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZqSpider.Core;
using ZqSpider.Core.Spiders;

namespace Example.Simple
{
    public class TMaill : ISpider
    {
        public string Name
        {
            get { return "TMaill"; }
        }

        public SpiderResponse Extract(Response response)
        {
            if (response.Request.Deep == 0)
            {
                var h = response.Html;
                var sellerId = Regex.Match(response.Html, @"sellerId=(\d+?)&").Groups[1].Value;
                var spuId = Regex.Match(response.Html, @"spuId=(\d+?)&").Groups[1].Value;
                var itemId = Regex.Match(response.Html, @"itemId=(\d+?)&").Groups[1].Value;

                var requests = new List<Request>();

                for (int i = 1; i < 2; i++)
                {
                    requests.Add(new Request()
                    {
                        Url = string.Format(@"https://rate.tmall.com/list_detail_rate.htm?itemId={0}&spuId={1}&sellerId={2}&currentPage={3}", itemId, spuId, sellerId, i),
                        AllowAutoRedirect = true
                    });
                }

                return SpiderResponse.Create(null, requests.ToArray());
            }
            else if (response.Request.Deep == 1)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Html);
                Console.WriteLine(response.Html);
            }
            return null;
        }

        public Request[] Requests
        {
            get
            {
                return new Request[]
                {
                    new Request()
                    {
                        Url = "https://detail.tmall.com/item.htm?spm=a1z10.3-b-s.w4011-14592463410.133.46f16db0JRp0ev&id=545583021796&rn=dd27adfc7f6327ec7744da220c8cfb68&abbucket=4",
                        AllowAutoRedirect = true
                    }
                };
            }
        }
    }
}
