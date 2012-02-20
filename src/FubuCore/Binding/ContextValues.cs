using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding
{
    public class ContextValues : IContextValues
    {
        public ContextValues(ISmartRequest request, List<Func<string, string>> namingStrategies, IRequestData rawData)
        {
            _request = request;
            _namingStrategies = namingStrategies;
            _rawData = rawData;
        }

        private readonly ISmartRequest _request;
        private readonly List<Func<string, string>> _namingStrategies;
        private readonly IRequestData _rawData;

        public object ValueAs(Type type, string name)
        {
            foreach (var naming in _namingStrategies)
            {
                var actualName = naming(name);
                var rawValue = _request.Value(type, actualName);
                if (rawValue != null)
                {
                    return rawValue;
                }
            }

            return null;
        }

        public bool ValueAs(Type type, string name, Action<object> continuation)
        {
            return _namingStrategies.Any(naming =>
            {
                string n = naming(name);
                return _request.Value(type, n, continuation);
            });
        }



        public T ValueAs<T>(string name)
        {
            T value = default(T);

            _namingStrategies.Any(naming =>
            {
                string n = naming(name);
                return _request.Value<T>(n, x => value = x);
            });

            return value;
        }

        public bool ValueAs<T>(string name, Action<T> continuation)
        {
            return _namingStrategies.Any(naming =>
            {
                var n = naming(name);
                return _request.Value(n, continuation);
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

            return value;
        }

        public bool RawValue(string name, Action<BindingValue> continuation)
        {
            _namingStrategies.Any(naming =>
            {
                string n = naming(name);
                return _rawData.Value(n, continuation);
            });

            return false;
        }
    }
}