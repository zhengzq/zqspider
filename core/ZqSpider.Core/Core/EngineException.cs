using System;

namespace ZqSpider.Core
{
    public class EngineException : Exception
    {
        public EngineException() : this("Engine") { }
        public EngineException(string message) : this(message, null) { }
        public EngineException(string message, Exception innerException) : base(message, innerException) { }
    }
}
