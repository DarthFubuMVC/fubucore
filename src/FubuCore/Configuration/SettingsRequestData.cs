using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SettingsRequestData : RequestData, IKeyValues
    {
        // This has to be sorted before you even get here
        public SettingsRequestData(IEnumerable<SettingsData> settingData) : base(settingData)
        {
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
            throw new NotSupportedException();
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            return Value(key, v => callback(v.RawKey, v.RawValue as string));
        }

        public static SettingsRequestData For(params SettingsData[] data)
        {
            return new SettingsRequestData(SettingsData.Order(data));
        }
    }
}