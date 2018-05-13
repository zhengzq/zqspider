using System.Threading.Tasks;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Core.Downloaders
{
    public interface IDownloader
    {
        Task<Response> AsyncFetch(Request request, ISpider spider);
        bool Overload { get; }
        bool Activing { get; }
        int Count { get;  }
    }
}
