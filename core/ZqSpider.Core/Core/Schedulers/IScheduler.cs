namespace ZqSpider.Core.Schedulers
{
    public interface IScheduler
    {
        Request Pop();
        bool Push(Request request);
        bool HasPendingRequests { get; }
        int Count { get; }
    }
}
