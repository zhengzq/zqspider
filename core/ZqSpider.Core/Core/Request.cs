using System.Collections.Generic;
using System.Text;

namespace ZqSpider.Core
{
    public class Request
    {
        public bool AllowAutoRedirect { get; set; } = false;
        public bool DontFilter { get; set; }
        public string Method { get; set; } = "GET";
        public string Url { get; set; }
        public string UserAgent { get; set; } = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public int Deep { get; internal set; }
        public Dictionary<string, object> Meta { get; set; }
        public Dictionary<string, string> Body { get; set; }
    }
}
