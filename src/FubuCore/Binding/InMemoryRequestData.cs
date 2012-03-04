using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class InMemoryRequestData : IRequestData
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public InMemoryRequestData()
        {
        }

        public InMemoryRequestData(IDictionary<string, object> values)
        {
            _values = new Cache<string, object>(values);
        }

        public object Value(string key)
        {
            return _values.Has(key) ? _values[key] : null;
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            if (_values.Has(key))
            {
                callback(new BindingValue
                {
                    RawKey = key,
                    RawValue = _values[key],
                    Source = "in memory"
                });
                return true;
            }

            return false;
        }

        public bool HasChildRequest(string key)
        {
            return _values.GetAllKeys().Any(x => x.StartsWith(key));
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            return new PrefixedRequestData(this, prefixOrChild);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            return EnumerateFlatRequestData.For(this, prefixOrChild);
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public void ReadData(string text)
        {
            StringPropertyReader.ForText(text).ReadProperties((key, value) => _values[key] = value);
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.GetAllKeys();
        }
    }
}