using System;

namespace ZqSpider.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create(string name);
        ILogger Create(Type type);
    }
}
