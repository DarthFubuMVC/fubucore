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
            return new SubstitutedRequestData(inner, new DictionaryKeyValues(_substitutions));
        }
    }
}