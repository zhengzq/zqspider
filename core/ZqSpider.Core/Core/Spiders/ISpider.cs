using System;

namespace ZqSpider.Core.Spiders
{
    public interface ISpider
    {
        string Name { get; }
        Request[] Requests { get; }
        SpiderResponse Extract(Response response);
    }
}
