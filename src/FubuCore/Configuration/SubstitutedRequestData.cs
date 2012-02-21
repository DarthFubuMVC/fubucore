using System;
using System.Collections.Generic;
using FubuCore.Binding;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SubstitutedRequestData : IRequestData
    {
        private readonly IRequestData _inner;
        private readonly IKeyValues _substitutions;

        public SubstitutedRequestData(IRequestData inner, IKeyValues substitutions)
        {
            _inner = inner;
            _substitutions = substitutions;
        }

        public object Value(string key)
        {
            var rawValue = _inner.Value(key);
            if (rawValue == null) return null;

            var parsedValue = rawValue.ToString();

            parsedValue = TemplateParser.Parse(parsedValue, _substitutions);

            return parsedValue;
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _inner.Value(key, o =>
            {
                o.RawValue = TemplateParser.Parse(o.RawValue.ToString(), _substitutions);
                callback(o);
            });
        }

        public bool HasAnyValuePrefixedWith(string key)
        {
            return _inner.HasAnyValuePrefixedWith(key);
        }

        public IRequestData GetSubRequest(string prefixOrChild)
        {
            var prefixedInner = _inner.GetSubRequest(prefixOrChild);
            return new SubstitutedRequestData(prefixedInner, _substitutions);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            return _inner.GetEnumerableRequests(prefixOrChild).Select(x => new SubstitutedRequestData(x, _substitutions));
        }
    }
}