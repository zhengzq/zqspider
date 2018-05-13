using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZqSpider.Core;
using ZqSpider.Core.Spiders;

namespace Example.Simple
{
    public class MovieSpider : ISpider
    {
        public string Name
        {
            get { return "Movie"; }
        }

        public Request[] Requests
        {
            get
            {
                var data = new List<Request>();
                for (int i = 1; i < 27; i++)
                {
                    data.Add(new Request()
                    {
                        Url = string.Format("http://www.ttmeiju.com/index.php/summary/index/p/{0}.html", i),
                        AllowAutoRedirect = true
                    });
                }
                return data.ToArray();
            }
        }

        public SpiderResponse Extract(Response response)
        {
            if (response.Request.Deep == 0)
            {
                return ExtractMovie(response);
            }
            else if (response.Request.Deep == 1)
            {
                return ExtractEpisode(response);
            }
            else
            {
                return null;
            }
        }

        private SpiderResponse ExtractMovie(Response response)
        {
            var requests = new List<Request>();
            var h = response.Html;
            var doc = new HtmlDocument();
            doc.LoadHtml(h);
            var trs = doc.DocumentNode.SelectNodes("//table[@class='latesttable']//tr[contains(@class,'Scontent1') or contains(@class,'Scontent')]");
            foreach (var item in trs)
            {
                var href = item.SelectSingleNode("td[2]/a").Attributes["href"].Value;
                if (string.IsNullOrEmpty(href))
                    continue;
                requests.Add(new Request()
                {
                    Url = string.Format("http://www.ttmeiju.com{0}", href),
                    AllowAutoRedirect = true
                });
            }
            return SpiderResponse.Create(null, requests.ToArray());
        }

        private SpiderResponse ExtractEpisode(Response response)
        {
            var moive = new Movie();
            var doc = new HtmlDocument();
            var item = new Item();
            var requests = new List<Request>();

            doc.LoadHtml(response.Html);
            moive.Title = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'newscontent')]//div[@class='hd']").InnerText.Replace("&nbsp;", "").Trim().Split('-')[0];
            var day = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'newscontent')]//div[@class='seedlink']/span[1]").InnerText.Trim();
            switch (day)
            {
                case "更新日：每周一":
                    moive.UpdateDay = 1;
                    break;
                case "更新日：每周二":
                    moive.UpdateDay = 2;
                    break;
                case "更新日：每周三":
                    moive.UpdateDay = 3;
                    break;
                case "更新日：每周四":
                    moive.UpdateDay = 4;
                    break;
                case "更新日：每周五":
                    moive.UpdateDay = 5;
                    break;
                case "更新日：每周六":
                    moive.UpdateDay = 6;
                    break;
                case "更新日：每周日":
                    moive.UpdateDay = 0;
                    break;
                default:
                    break;
            }
            moive.Summary = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'newscontent')]//div[@class='newstxt']/p").InnerText;
            //var seasons = doc.DocumentNode.SelectNodes("//div[@class='seasonitem']/h3");
            //foreach (var season in seasons)
            //{
            //    requests.Add(new Request()
            //    {
            //        Method = "POST",
            //        Url = "http://www.ttmeiju.com/index.php/meiju/get_episodies.html",
            //        AllowAutoRedirect = true
            //    });
            //}
            item.Data["url"] = response.Request.Url;
            item.Data["moive"] = moive;
            return SpiderResponse.Create(item, requests.ToArray());
        }
    }

    public class Movie
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<Episode> Episodes { get; set; }
        public int UpdateDay { get; set; }
        public class Episode
        {
            public int Number { get; set; }
            public int Season { get; set; }
            public string Name { get; set; }
            public int Size { get; set; }
            public string Img { get; set; }
            public List<Source> Sources { get; set; }
            public class Source
            {
                public string Name { get; set; }
                public string Link { get; set; }
            }
        }
    }
}
