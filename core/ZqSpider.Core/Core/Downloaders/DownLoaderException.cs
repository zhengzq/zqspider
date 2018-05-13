using System;

namespace ZqSpider.Core.Downloaders
{
    public class DownLoaderException : Exception
    {
        public DownLoaderException() : this("DownLoader") { }
        public DownLoaderException(string message) : this(message, null) { }
        public DownLoaderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
