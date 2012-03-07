using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.Values;
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

        public bool HasChildRequest(string key)
        {
            return _inner.HasChildRequest(key);
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            var prefixedInner = _inner.GetChildRequest(prefixOrChild);
            return new SubstitutedRequestData(prefixedInner, _substitutions);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            return _inner.GetEnumerableRequests(prefixOrChild).Select(x => new SubstitutedRequestData(x, _substitutions));
        }

        public void AddValues(string name, IKeyValues values)
        {
            _inner.AddValues(name, values);
        }

        public void AddValues(IValueSource source)
        {
            _inner.AddValues(source);
        }

        public IValueSource ValuesFor(string nameOrProvenance)
        {
            return _inner.ValuesFor(nameOrProvenance);
        }

        public void WriteReport(IValueReport report)
        {
            _inner.WriteReport(report);
        }
    }
}