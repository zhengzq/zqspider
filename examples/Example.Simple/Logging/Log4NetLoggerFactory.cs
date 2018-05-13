using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using ZqSpider.Logging;

namespace Example.Simple
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerRepository _repository;
        private readonly string _repositoryName = "LOG4NET";

        public Log4NetLoggerFactory(string configFile = "log4net.config")
        {
            var file = new FileInfo(configFile);
            _repository = LogManager.CreateRepository(_repositoryName);
            if (!file.Exists)
            {
                file = new FileInfo(Path.Combine(AppContext.BaseDirectory, configFile));
            }

            if (file.Exists)
            {
                XmlConfigurator.Configure(_repository, file);
            }
            else
            {
                BasicConfigurator.Configure(_repository);
            }
        }

        public ILogger Create(string name)
        {
            return new Log4NetLogger(LogManager.GetLogger(_repositoryName, name));
        }
        public ILogger Create(Type type)
        {
            return new Log4NetLogger(LogManager.GetLogger(type));
        }
    }
}
