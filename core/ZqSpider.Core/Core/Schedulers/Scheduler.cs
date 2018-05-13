using ZqSpider.Core.Queues;

namespace ZqSpider.Core.Schedulers
{
    public class Scheduler : IScheduler
    {
        private readonly IDupeFilter _dupeFilter;
        private readonly IRequestQueue _cacheQueue;
        private readonly Setting _setting;

        public Scheduler(IDupeFilter dupeFilter, Setting setting)
        {
            this._dupeFilter = dupeFilter;
            this._cacheQueue = new MemoryQueue();
            this._setting = setting;
        }

        public bool HasPendingRequests
        {
            get { return this._cacheQueue.Count > 0; }
        }

        public int Count
        {
            get
            {
                return this._cacheQueue.Count;
            }
        }

        public bool Push(Request request)
        {
            if (request == null)
                return false;

            if (!request.DontFilter)
                if (_dupeFilter.Seen(request))
                    return false;

            _cacheQueue.Enqueue(request);
            return true;
        }

        public Request Pop()
        {
            if (_cacheQueue.Count == 0)
                return null;
            return _cacheQueue.Dequeue();
        }
    }
}
