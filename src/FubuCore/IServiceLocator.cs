using System;
using FubuCore.Conversion;
using FubuCore.Util;

namespace FubuCore
{
    public interface IServiceLocator
    {
        T GetInstance<T>();
        object GetInstance(Type type);
        T GetInstance<T>(string name);
    }

    public class InMemoryServiceLocator : IServiceLocator
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();
        private readonly Cache<string, object> _namedServices = new Cache<string, object>();

        public InMemoryServiceLocator()
        {
            Add<IObjectConverter>(new ObjectConverter(this, new ConverterLibrary()));
        }

        public void Add<T>(T service)
        {
            _services[typeof(T)] = service;
        }
        public void Add<T>(T service, string name)
        {
            _namedServices[name] = service;
        }

        public T GetInstance<T>()
        {
            return (T) _services[typeof (T)];
        }

        public T GetInstance<T>(string name)
        {
            return (T) _namedServices[name];
        }

        public object GetInstance(Type type)
        {
            return _services[type];
        }
    }
}