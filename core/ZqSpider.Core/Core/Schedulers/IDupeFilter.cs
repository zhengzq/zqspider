namespace ZqSpider.Core.Schedulers
{
    public interface IDupeFilter
    {
        bool Seen(Request request);
    }
}
