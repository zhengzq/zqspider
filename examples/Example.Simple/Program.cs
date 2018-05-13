using System;
using ZqSpider;
using ZqSpider.Configurations;
using ZqSpider.Core;

namespace Example.Simple
{
    public class Program
    {
        public static void Main(string[] args)
        {
            KillAllPhantomjs();
            var configuration = CreateConfiguration();
            var executer = new Executer(configuration);
            var setting = new Setting()
            {
                PiplelineNames = new string[] { "Store" },
                TotalConcurrency = 10
            };
            executer.AddSpider(new TMaill(), setting);
            executer.AddSpider(new MovieSpider(), setting);
            executer.AddSpider(new SimpleSpider(), setting);

            var spiderName = default(string);
            if (args != null && args.Length > 0)
            {
                spiderName = args[0].Trim();
            }
            else
            {
                Console.WriteLine("please input spider name. empty will run all spider.");
                spiderName = Console.ReadLine().Trim();
            }

            if (!string.IsNullOrEmpty(spiderName))
            {
                executer.Start(spiderName);
            }
            else
            {
                executer.Start();
            }

            Console.Read();
        }

        private static Configuration CreateConfiguration()
        {
            var configuration = Configuration.Instance.RegisterComponents(c =>
            {
                c.RegisterLogger(typeof(Log4NetLogger), typeof(Log4NetLoggerFactory));
                c.RegisterItemPipeline("Store", typeof(StoreItemPipeline));
                //c.RegisterDownloadHandler("DefaultDownloadHandlers", new Dictionary<string, DownloadHandler>()
                //{
                //    { "http",new SeleniumDownloadHandler() },
                //    { "https",new SeleniumDownloadHandler() },
                //});
            });
            return configuration;
        }

        private static void KillAllPhantomjs()
        {
            foreach (var item in System.Diagnostics.Process.GetProcessesByName("phantomjs"))
                item.Kill();
        }
    }
}
