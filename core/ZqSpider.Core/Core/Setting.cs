namespace ZqSpider.Core
{
    public class Setting
    {
        public string SchedulerName { get; set; } = "DefaultScheduler";

        public string DownloaderName { get; set; } = "DefaultDownloader";

        public string DownloadHandlersName { get; set; } = "DefaultDownloadHandlers";

        public int TotalConcurrency { get; set; } = 10;

        public string[] PiplelineNames { get; set; } = { };

        public string[] DownloaderMiddlewareNames { get; set; } = { };
    }
}
