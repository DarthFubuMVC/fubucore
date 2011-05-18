using System;
using System.Collections.Generic;
using FubuCore.Binding;

namespace FubuCore.Configuration
{
    public class SubstitutedSettingsProvider : SettingsProvider
    {
        private readonly IDictionary<string, string> _substitutions;

        public SubstitutedSettingsProvider(IObjectResolver resolver, IEnumerable<SettingsData> settings, IDictionary<string, string> substitutions)
            : base(resolver, settings)
        {
            _substitutions = substitutions;
        }

        protected override IRequestData createRequestData(Type settingsType)
        {
            var inner = base.createRequestData(settingsType);
            return new SubstitutedRequestData(inner, _substitutions);
        }
    }

    public class SubstitutedRequestData : IRequestData
    {
        private readonly IRequestData _inner;
        private readonly IDictionary<string, string> _substitutions;

        public SubstitutedRequestData(IRequestData inner, IDictionary<string, string> substitutions)
        {
            _inner = inner;
            _substitutions = substitutions;
        }

        public object Value(string key)
        {
            var rawValue = _inner.Value(key);
            if (rawValue == null) return null;

            return TemplateParser.Parse(rawValue.ToString(), _substitutions);
        }

        public bool Value(string key, Action<object> callback)
        {
            return _inner.Value(key, o =>
            {
                var substitutedValue = TemplateParser.Parse(o.ToString(), _substitutions);
                callback(substitutedValue);
            });
        }

        public bool HasAnyValuePrefixedWith(string key)
        {
            return _inner.HasAnyValuePrefixedWith(key);
        }
    }
}