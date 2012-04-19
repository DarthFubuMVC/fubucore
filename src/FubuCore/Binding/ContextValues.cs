using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;

namespace FubuCore.Binding
{
    public class ContextValues : IContextValues
    {
        public ContextValues(IObjectConverter converter, List<Func<string, string>> namingStrategies, IRequestData rawData, IBindingLogger logger)
        {
            _converter = converter;
            _namingStrategies = namingStrategies;
            _rawData = rawData;
            _logger = logger;
        }

        private readonly IObjectConverter _converter;
        private readonly List<Func<string, string>> _namingStrategies;
        private readonly IRequestData _rawData;
        private readonly IBindingLogger _logger;

        public T ValueAs<T>(string name)
        {
            var bindingValue = RawValue(name);
            if (bindingValue == null || bindingValue.RawValue == null) return default(T);

            return _converter.FromString<T>(bindingValue.RawValue.ToString());
        }

        public object ValueAs(Type type, string name)
        {
            var bindingValue = RawValue(name);
            if (bindingValue == null || bindingValue.RawValue == null) return null;

            return _converter.FromString(bindingValue.RawValue.ToString(), type);
        }

        public bool ValueAs<T>(string name, Action<T> continuation)
        {
            return RawValue(name, value =>
            {
                if (value.RawValue != null)
                {
                    var convertedValue = _converter.FromString<T>(value.RawValue.ToString());
                    continuation(convertedValue);
                }
            });
        }

        public bool ValueAs(Type type, string name, Action<object> continuation)
        {
            return RawValue(name, value =>
            {
                if (value.RawValue != null)
                {
                    var convertedValue = _converter.FromString(value.RawValue.ToString(), type);
                    continuation(convertedValue);
                }
            });
        }

        public BindingValue RawValue(string name)
        {
            BindingValue value = null;
            _namingStrategies.Any(naming =>
            {
                string n = naming(name);
                return _rawData.Value(n, x => value = x);
            });

            if (value != null)
            {
                _logger.UsedValue(value);
            }

            return value;
        }

        public bool RawValue(string name, Action<BindingValue> continuation)
        {
            return _namingStrategies.Any(naming =>
            {
                string n = naming(name);
                return _rawData.Value(n, value =>
                {
                    _logger.UsedValue(value);
                    continuation(value);
                });
            });
        }
    }
}