using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    [MarkedForTermination("Can kill the while shebang?, maybe just subclass DictionaryValueSource")]
    public class SettingsData
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public SettingsData() : this(SettingCategory.core)
        {
        }

        public SettingsData(SettingCategory category)
        {
            Category = category;
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        [MarkedForTermination("Becomes just 'name' in the diagnostics")]
        public string Provenance { get; set; }

        public SettingCategory Category { get; set; }

        public IEnumerable<string> GetKeys()
        {
            return _values.GetAllKeys();
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

        public object Get(string key)
        {
            return _values[key];
        }

        public void Read(string text)
        {
            StringPropertyReader.ReadLine(text, (key, value) => _values[key] = value);
        }

        public static SettingsData ReadFromFile(SettingCategory category, string file)
        {
            var data = new SettingsData(category){
                Provenance = file
            };

            StringPropertyReader.ForFile(file).ReadProperties((key, value) => data._values[key] = value);

            return data;
        }

        public override string ToString()
        {
            return string.Format("Settings Data, category {0}, provenance: {1}", Category, Provenance ?? "unknown");
        }
    }
}