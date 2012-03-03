using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    
    public class SettingsRequestData : IRequestData, IKeyValues
    {
        private readonly SettingsStep _profileStep;
        private readonly SettingsStep _environmentStep;
        private readonly SettingsStep _packageStep;
        private readonly SettingsStep _coreStep;
        
        private readonly SettingsStep[] _steps;


        public SettingsRequestData(IEnumerable<SettingsData> settingData)
        {
            _profileStep = new SettingsStep(settingData.Where(x=>x.Category == SettingCategory.profile));
            _environmentStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.environment));
            _packageStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.package));
            _coreStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.core));

            _steps = new []{_profileStep, _environmentStep, _packageStep, _coreStep};
        }

        public object Value(string key)
        {
            BindingValue returnValue = null;

            Value(key, o => returnValue = o);

            return (returnValue.RawValue ?? string.Empty).ToString();
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _steps.Any(x => x.Value(key, callback));
        }

        

        public bool HasChildRequest(string key)
        {
            return _steps.Any(x => x.HasAnyValuePrefixedWith(key));
        }

        public static SettingsRequestData For(params SettingsData[] data)
        {
            return new SettingsRequestData(data);
        }

        public class SettingsStep
        {
            private readonly IEnumerable<SettingsData> _settingData;

            public SettingsStep(IEnumerable<SettingsData> settingData)
            {
                _settingData = settingData;
            }

            public IEnumerable<string> AllKeys
            {
                get { return _settingData.SelectMany(data => data.GetKeys()); }
            }

            public bool HasAnyValuePrefixedWith(string key)
            {
                return _settingData.Any(x => x.GetKeys().Any(k => k.StartsWith(key)));
            }

            public bool Value(string key, Action<BindingValue> callback)
            {
                var data = _settingData.FirstOrDefault(x => x.Has(key));
                if (data == null) return false;

                callback(new BindingValue{
                    RawValue = data.Get(key),
                    RawKey = key,
                    Source = data.ToString() // SettingsData should now show you the provenance and category in here
                });

                return true;
            }

            public SettingDataSource DiagnosticValueOf(string key)
            {
                var setting = _settingData.FirstOrDefault(x => x.Has(key));
                return setting == null ? null : new SettingDataSource(){
                    Key = key, Provenance = setting.Provenance, Value = setting.Get(key)
                };
            }
        }

        public IEnumerable<SettingDataSource> CreateDiagnosticReport()
        {
            return _steps.SelectMany(step => step.AllKeys)
                .Distinct().OrderBy(x=>x)
                .Select(diagnosticSourceForKey);
        }

        private SettingDataSource diagnosticSourceForKey(string key)
        {
            return _steps.FirstValue(x => x.DiagnosticValueOf(key));
        }

        public bool Has(string key)
        {
            return Value(key, o => { });
        }

        public string Get(string key)
        {
            return Value(key) as string;
        }

        public IEnumerable<string> GetKeys()
        {
            return _steps.SelectMany(s => s.AllKeys);
        }

        // TODO -- this won't stay like this
        public bool ForValue(string key, Action<string, string> callback)
        {
            return Value(key, v => callback(v.RawKey, v.RawValue as string));
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            return new PrefixedRequestData(this, prefixOrChild);
        }

        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            throw new NotImplementedException();
        }
    }
}