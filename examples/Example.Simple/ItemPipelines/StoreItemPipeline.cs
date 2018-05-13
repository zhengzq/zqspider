using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZqSpider.Core;
using ZqSpider.Core.Spiders;

namespace Example.Simple
{
    public class StoreItemPipeline : ItemPipeline
    {
        public override Task Process(Item item, ISpider spider)
        {
            switch (spider.Name)
            {
                case "Simple":
                case "Movie":
                case "TMaill":
                default:
                    Console.WriteLine("-----------------------");
                    foreach (var n in item.Data)
                    {
                        Console.WriteLine(string.Format("{0}:{1}",n.Key,n.Value));
                    }
                    Console.WriteLine("-----------------------");
                    break;
            }
            return base.Process(item, spider);
        }
    }
}
