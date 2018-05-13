using System;
using System.Threading.Tasks;
using ZqSpider.Core;
using ZqSpider.Core.Downloaders;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Selenium
{
    public class SeleniumDownloadHandler : DownloadHandler
    {
        public override async Task<Response> Download(Request request, Response response, ISpider spider)
        {
            try
            {
                var html = await Task.Run<string>(() =>
                {
                    using (var driver = WebDriverPool.Get())
                    {
                        driver.Navigate().GoToUrl(request.Url);
                        driver.Manage().Window.Size = new System.Drawing.Size(1024, 5000);
                        //Thread.Sleep(5 * 1000);
                        return driver.PageSource;
                    }
                });

                response.Html = html;
                return response;
            }
            catch (Exception ex)
            {
                throw new DownLoaderException(string.Format("{0}\n URL {1}\n {2}", ex.Message, request.Url, ex.ToString()));
            }
        }
    }
}
