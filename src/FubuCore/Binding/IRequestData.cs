using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding
{
    public interface IRequestData
    {
        object Value(string key);
        bool Value(string key, Action<object> callback);
        bool HasAnyValuePrefixedWith(string key);
        IEnumerable<string> GetKeys();
    }

    public abstract class RequestDataBase : IRequestData
    {
        private readonly Lazy<IEnumerable<string>> _allKeys;

        protected RequestDataBase()
        {
            _allKeys = new Lazy<IEnumerable<string>>(GetKeys);
        }

        public object Value(string key)
        {
            return hasValue(key) ? fetch(key) : null;
        }

        public virtual bool Value(string key, Action<object> callback)
        {
            if (hasValue(key))
            {
                callback(fetch(key));
                return true;
            }

            return false;
        }

        protected abstract object fetch(string key);
        protected virtual bool hasValue(string key)
        {
            return _allKeys.Value.Contains(key);
        }

        public virtual bool HasAnyValuePrefixedWith(string key)
        {
            return _allKeys.Value.Any(x => x.StartsWith(key));
        }

        public abstract IEnumerable<string> GetKeys();
    }
}