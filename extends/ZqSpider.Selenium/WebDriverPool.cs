using OpenQA.Selenium.PhantomJS;
using System;

namespace ZqSpider.Selenium
{
    public class WebDriverPool
    {
        internal static PhantomJSDriver Get()
        {
            try
            {
                var service = PhantomJSDriverService.CreateDefaultService(AppContext.BaseDirectory);
                service.LoadImages = false;
                service.IgnoreSslErrors = true;
                service.LocalToRemoteUrlAccess = true;
                service.HideCommandPromptWindow = true;
                var driver = new PhantomJSDriver(service);
                return driver;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
