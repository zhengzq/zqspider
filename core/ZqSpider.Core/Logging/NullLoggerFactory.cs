using System;

namespace ZqSpider.Logging
{
    public class NullLoggerFactory : ILoggerFactory
    {
        private static readonly NullLogger _logger = new NullLogger();

        public ILogger Create(string name)
        {
            return _logger;
        }
        public ILogger Create(Type type)
        {
            return _logger;
        }
    }
}
