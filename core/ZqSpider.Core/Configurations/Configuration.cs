using System;
using System.Text;

namespace ZqSpider.Configurations
{
    public class Configuration
    {
        private static Configuration _instance;

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Configuration();
                return _instance;
            }
        }

        public Configuration()
        {
            this.ComponentManager = new ComponentManager();
        }

        internal ComponentManager ComponentManager { get; private set; }

        public Configuration RegisterComponents(Action<ComponentManager> action = null)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.ComponentManager.RegisterDefault();
            action?.Invoke(this.ComponentManager);

            this.ComponentManager.Build();
            return this;
        }
    }
}
