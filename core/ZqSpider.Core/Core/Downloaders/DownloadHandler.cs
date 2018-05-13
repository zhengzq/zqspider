using System.Threading.Tasks;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Core.Downloaders
{
    public abstract class DownloadHandler
    {
        public abstract Task<Response> Download(Request request, Response response, ISpider spider);
    }
}
