namespace ZqSpider.Core.Queues
{
    public interface IRequestQueue
    {
        int Count { get; }
        void Enqueue(Request request);
        Request Dequeue();
    }
}
