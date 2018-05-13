using System;
using System.Collections.Generic;
using Autofac;
using ZqSpider.Core;
using ZqSpider.Core.Downloaders;
using ZqSpider.Core.Queues;
using ZqSpider.Core.Schedulers;
using ZqSpider.Logging;

namespace ZqSpider.Configurations
{
    public class ComponentManager
    {
        private readonly ContainerBuilder _builder;
        private IContainer _container;
        private Dictionary<string, DownloadHandler> _defaultDownloadHandlers = new Dictionary<string, DownloadHandler>
        {
            { SchemeStandard.Http, new HttpDownloadHandler() },
            { SchemeStandard.Https, new HttpDownloadHandler() }
        };

        public ComponentManager()
        {
            this._builder = new ContainerBuilder();
        }

        internal ComponentManager RegisterDefault()
        {
            _builder.RegisterType<NullLogger>().As<ILogger>();
            _builder.RegisterType<NullLoggerFactory>().As<ILoggerFactory>();
            _builder.RegisterType<Engine>();
            _builder.RegisterType<Downloader>().Named<IDownloader>("DefaultDownloader");
            _builder.RegisterType<Scheduler>().Named<IScheduler>("DefaultScheduler");
            _builder.RegisterType<DefaultDupeFilter>().As<IDupeFilter>();
            _builder.Register(c => _defaultDownloadHandlers).Named<Dictionary<string, DownloadHandler>>("DefaultDownloadHandlers");
            return this;
        }

        public void RegisterDownloadHandler(string name, Dictionary<string, DownloadHandler> dictionary)
        {
            _builder.Register(c =>
            {
                var dic = new Dictionary<string, DownloadHandler>();
                foreach (var item in _defaultDownloadHandlers)
                    dic[item.Key] = item.Value;
                foreach (var item in dictionary)
                    dic[item.Key] = item.Value;
                return dic;
            }).Named<Dictionary<string, DownloadHandler>>(name);
        }

        public ComponentManager RegisterLogger(Type logger, Type loggerFactory)
        {
            _builder.RegisterType(logger).As<ILogger>().SingleInstance();
            _builder.RegisterType(loggerFactory).As<ILoggerFactory>().SingleInstance();
            return this;
        }

        public ComponentManager RegisterDownloader(string name, Type type)
        {
            _builder.RegisterType(type).Named<IDownloader>(name);
            return this;
        }

        public ComponentManager RegisterItemPipeline(string name, Type type)
        {
            _builder.RegisterType(type).Named<ItemPipeline>(name);
            return this;
        }

        public ComponentManager RegisterQueue(string name, Type type)
        {
            _builder.RegisterType(type).Named<IRequestQueue>(name);
            return this;
        }

        public ComponentManager RegisterScheduler(string name, Type type)
        {
            _builder.RegisterType(type).Named<IScheduler>(name);
            return this;
        }

        public ComponentManager RegisterDownloaderMiddleware(string name, Type type)
        {
            _builder.RegisterType(type).Named<DownloaderMiddleware>(name);
            return this;
        }

        public T Resolve<T>(string name)
        {
            if (_container == null)
                throw new InvalidOperationException("ComponentManager was not build");
            return _container.ResolveNamed<T>(name);
        }

        public T Resolve<T>()
        {
            if (_container == null)
                throw new InvalidOperationException("ComponentManager was not build");
            return _container.Resolve<T>();
        }

        internal T Resolve<T, TParameter>(string name, TParameter parameter)
        {
            if (_container == null)
                throw new InvalidOperationException("ComponentManager was not build");
            return _container.ResolveNamed<T>(name, new TypedParameter(typeof(TParameter), parameter));
        }

        internal T Resolve<T, TParameter1, Tparameter2>(string name, TParameter1 p1, Tparameter2 p2)
        {
            if (_container == null)
                throw new InvalidOperationException("ComponentManager was not build");
            return _container.ResolveNamed<T>(name, new TypedParameter(typeof(TParameter1), p1), new TypedParameter(typeof(Tparameter2), p2));
        }

        internal T Resolve<T, TParameter1, Tparameter2, Tparameter3>(string name, TParameter1 p1, Tparameter2 p2, Tparameter3 p3)
        {
            if (_container == null)
                throw new InvalidOperationException("ComponentManager was not build");
            return _container.ResolveNamed<T>(name, new TypedParameter(typeof(TParameter1), p1), new TypedParameter(typeof(Tparameter2), p2), new TypedParameter(typeof(Tparameter3), p3));
        }

        internal void Build()
        {
            this._container = _builder.Build();
        }
    }
}
