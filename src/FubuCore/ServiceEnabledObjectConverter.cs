using System;
using System.Collections.Generic;
using FubuCore.Conversion;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore
{
    public class ServiceEnabledObjectConverter : ObjectConverter
    {
        private readonly IServiceLocator _locator;

        public ServiceEnabledObjectConverter(IServiceLocator locator, IEnumerable<IObjectConverterFamily> families)
        {
            _locator = locator;
            families.Each(RegisterConverterFamily);
        }

        protected override object getService(Type type)
        {
            return _locator.GetInstance(type);
        }

        public void RegisterConverter<T, TService>(Func<TService, string, T> converter)
        {
            RegisterConverter<T>(text => converter(_locator.GetInstance<TService>(), text));
        }
    }
}