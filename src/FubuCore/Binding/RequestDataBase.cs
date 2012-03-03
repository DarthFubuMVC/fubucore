using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding
{
    public abstract class RequestDataBase : IRequestData
    {
        private readonly Lazy<IEnumerable<string>> _allKeys;

        protected RequestDataBase()
        {
            _allKeys = new Lazy<IEnumerable<string>>(GetKeys);
        }

        public object Value(string key)
        {
            return Has(key) ? Get(key) : null;
        }

        protected abstract string source
        { 
            get;
        }

        public virtual bool Value(string key, Action<BindingValue> callback)
        {
            if (Has(key))
            {
                callback(new BindingValue{
                    RawKey = key,
                    RawValue = Get(key),
                    Source = source
                });
                return true;
            }

            return false;
        }

        protected abstract object Get(string key);
        protected virtual bool Has(string key)
        {
            return _allKeys.Value.Contains(key);
        }

        public virtual bool HasChildRequest(string key)
        {
            return _allKeys.Value.Any(x => x.StartsWith(key));
        }

        public abstract IEnumerable<string> GetKeys();

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            return new PrefixedRequestData(this, prefixOrChild);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            return EnumerateFlatRequestData.For(this, prefixOrChild);
        }
    }
}