namespace ZqSpider.Core.Schedulers
{
    public class DefaultDupeFilter : IDupeFilter
    {
        public BloomFilter<string> _bf = new BloomFilter<string>(5227520, 5227520,3);
        public bool Seen(Request request)
        {
            if (_bf.Contains(request.Url))
                return true;

            _bf.Add(request.Url);
            return false;
        }
    }
}
