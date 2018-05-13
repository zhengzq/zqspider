using System.Threading.Tasks;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Core
{
    public class ItemPipeline
    {
        public virtual Task Process(Item item, ISpider spider)
        {
            return Task.Delay(0);
        }
    }
}
