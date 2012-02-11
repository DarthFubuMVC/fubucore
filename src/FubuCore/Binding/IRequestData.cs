using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding
{
    public interface IRequestData
    {
        object Value(string key);
        bool Value(string key, Action<RequestSource> callback);
        bool HasAnyValuePrefixedWith(string key);
        IEnumerable<string> GetKeys();

        IRequestData GetSubRequest(string prefixOrChild);

    }

    public class RequestSource
    {
        public string RawKey { get; set; }
        public string Source { get; set; }
        public object RawValue { get; set; }

        public bool Equals(RequestSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.RawKey, RawKey) && Equals(other.Source, Source) && Equals(other.RawValue, RawValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestSource)) return false;
            return Equals((RequestSource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (RawKey != null ? RawKey.GetHashCode() : 0);
                result = (result*397) ^ (Source != null ? Source.GetHashCode() : 0);
                result = (result*397) ^ (RawValue != null ? RawValue.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("RawKey: {0}, Source: {1}, RawValue: {2}", RawKey, Source, RawValue);
        }
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

        protected abstract string source
        { 
            get;
        }

        public virtual bool Value(string key, Action<RequestSource> callback)
        {
            if (hasValue(key))
            {
                callback(new RequestSource{
                    RawKey = key,
                    RawValue = fetch(key),
                    Source = source
                });
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

        public IRequestData GetSubRequest(string prefixOrChild)
        {
            return new PrefixedRequestData(this, prefixOrChild);
        }
    }
}