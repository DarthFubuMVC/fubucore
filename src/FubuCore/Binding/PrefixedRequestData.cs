using System;
using System.Collections.Generic;

namespace FubuCore.Binding
{
    [MarkedForTermination("Goes away soon")]
    public class PrefixedRequestData : IRequestData
    {
        private readonly IRequestData _inner;
        private readonly string _prefix;

        public PrefixedRequestData(IRequestData inner, string prefix)
        {
            _inner = inner;
            _prefix = prefix;
        }

        public object Value(string key)
        {
            return _inner.Value(_prefix + key);
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _inner.Value(_prefix + key, callback);
        }

        public bool HasChildRequest(string key)
        {
            return _inner.HasChildRequest(_prefix + key);
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            return new PrefixedRequestData(_inner, _prefix + prefixOrChild);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            return _inner.GetEnumerableRequests(_prefix + prefixOrChild);
        }
    }
}