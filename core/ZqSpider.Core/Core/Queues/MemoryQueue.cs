using System.Collections.Concurrent;

namespace ZqSpider.Core.Queues
{
    public class MemoryQueue : IRequestQueue
    {
        private readonly ConcurrentQueue<Request> _queue = new ConcurrentQueue<Request>();

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Enqueue(Request request)
        {
            if (request == null)
                return;
            _queue.Enqueue(request);
        }

        public Request Dequeue()
        {
            return _queue.TryDequeue(out Request request) ? request : default(Request);
        }
    }
}