using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore.Binding;
using FubuCore.Binding.Values;

namespace FubuCore.Configuration
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly Lazy<SettingsRequestData> _requestData;
        private readonly IObjectResolver _resolver;
        private readonly Lazy<IEnumerable<SettingsData>> _settings;
        private readonly Lazy<SubstitutedRequestData> _substitutedData;

        private SettingsProvider(IObjectResolver resolver, IEnumerable<SettingsData> settings)
            : this(resolver, new ISettingsSource[]{new SettingsSource(settings)})
        {
        }

        public SettingsProvider(IObjectResolver resolver, IEnumerable<ISettingsSource> sources)
        {
            _resolver = resolver;

            _settings = new Lazy<IEnumerable<SettingsData>>(() =>
            {
                var allSettings = sources.SelectMany(x => x.FindSettingData()).ToArray();
                return SettingsData.Order(allSettings);
            });

            _requestData = new Lazy<SettingsRequestData>(() => new SettingsRequestData(_settings.Value));

            _substitutedData = new Lazy<SubstitutedRequestData>(() => new SubstitutedRequestData(_requestData.Value, _requestData.Value));
        }

        public T SettingsFor<T>() where T : class, new()
        {
            return (T) SettingsFor(typeof (T));
        }

        public object SettingsFor(Type settingsType)
        {
            var prefixedData = createRequestData(settingsType);

            var result = _resolver.BindModel(settingsType, prefixedData);
            result.AssertNoProblems(settingsType);

            return result.Value;
        }

        public object SettingFor(string key)
        {
            return _substitutedData.Value.Value(key);
        }

        protected virtual IRequestData createRequestData(Type settingsType)
        {
            // TODO -- throw if not exists?
            return _substitutedData.Value.GetChildRequest(settingsType.Name);
        }

        public IEnumerable<SettingDataSource> CreateDiagnosticReport()
        {
            var report = new ValueDiagnosticReport();

            // TODO -- watch this, probably duplicated code
            _settings.Value.Each(x =>
            {
                report.StartSource(x);
                x.WriteReport(report);
                report.EndSource();
            });

            return report.AllValues().Select(x => new SettingDataSource{
                Key = x.Key,
                Provenance = x.First().Source,
                Value = x.First().Value
            });
        }

        public IEnumerable<SettingDataSource> CreateResolvedDiagnosticReport()
        {
            var settingsData = _requestData.Value;

            return CreateDiagnosticReport().Select(s => new SettingDataSource{
                Key = s.Key,
                Value = TemplateParser.Parse(s.Value, settingsData),
                Provenance = s.Provenance
            });
        }

        public static SettingsProvider For(params SettingsData[] data)
        {
            return new SettingsProvider(ObjectResolver.Basic(), data);
        }

        public void AssertAllSubstitutionsCanBeResolved()
        {
            var report = CreateDiagnosticReport();
            var substitutions = report.SelectMany(x => TemplateParser.GetSubstitutions(x.Value)).Distinct();

            var missing = substitutions.Where(x => !report.Any(s => s.Key == x));
            if (missing.Any())
            {
                throw new SettingProviderException("Missing required values for " + missing.Join(", "));
            }
        }
    }

    [Serializable]
    public class SettingProviderException : Exception
    {
        public SettingProviderException(string message) : base(message)
        {
        }

        public SettingProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SettingProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}