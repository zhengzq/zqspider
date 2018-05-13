namespace ZqSpider.Core
{
    public class Response 
    {
        public string Html { get; set; }
        public Request Request { get; set; }
        public string RootUrl { get; internal set; }
        public byte[] Body { get; internal set; }
    }
}
