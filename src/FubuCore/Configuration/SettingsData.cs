using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SettingsData
    {
        private readonly Cache<string, string> _values = new Cache<string, string>();

        public SettingsData() : this(SettingCategory.core)
        {
        }

        public SettingsData(SettingCategory category)
        {
            Category = category;
        }

        public string this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public string Provenance { get; set; }

        public SettingCategory Category { get; set; }

        public IEnumerable<string> AllKeys
        {
            get { return _values.GetAllKeys(); }
        }

        public SettingsData With(string key, string value)
        {
            _values[key] = value;
            return this;
        }

        public bool Has(string key)
        {
            return _values.Has(key);
        }

        public string Get(string key)
        {
            return _values[key];
        }
    }
}