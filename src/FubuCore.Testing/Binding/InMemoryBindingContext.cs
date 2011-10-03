using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore.Testing.Binding
{
    public class InMemoryBindingContext : BindingContext
    {
        private readonly InMemoryRequestData _data;
        private readonly StubServiceLocator _services;

        public InMemoryBindingContext() : this(new InMemoryRequestData(), new StubServiceLocator())
        {
        }

        private InMemoryBindingContext(InMemoryRequestData data, StubServiceLocator services)
            : base(data, services, new NulloBindingLogger())
        {
            _services = services;
            _data = data;
        }

        public void RegisterService<T>(T @object)
        {
            _services.Services[typeof (T)] = @object;
        }

        public InMemoryBindingContext WithData(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public InMemoryBindingContext WithPropertyValue(object value)
        {
            PropertyValue = value;
            return this;
        }

        public InMemoryRequestData Data { get { return _data; } }

        public object this[string key] { get { return _data[key]; } set { _data[key] = value; } }
    }

    public class StubServiceLocator : IServiceLocator
    {
        public readonly Cache<Type, object> Services = new Cache<Type, object>();

        public object GetService(Type serviceType)
        {
            return Services[serviceType];
        }

        public object GetInstance(Type serviceType)
        {
            return Services[serviceType];
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            return (TService) Services[typeof(TService)];
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }
    }
}