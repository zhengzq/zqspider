using System.Threading.Tasks;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Core.Downloaders
{
    public class DownloaderMiddleware
    {
        public virtual async Task<Response> ProcessResponse(Request request, Response response, ISpider spider)
        {
            return await Task.FromResult<Response>(response);
        }

        public virtual async Task<Request> ProcessRequest(Request request, ISpider spider)
        {
            return await Task.FromResult<Request>(request);
        }
    }
}
