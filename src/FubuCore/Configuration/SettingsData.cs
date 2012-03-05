using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SettingsData : DictionaryValueSource
    {
        public SettingsData() : this(SettingCategory.core)
        {
        }

        public SettingsData(SettingCategory category) : base(new Dictionary<string, object>())
        {
            Category = category;
        }

        public SettingCategory Category { get; set; }

        public SettingsData With(string key, string value)
        {
            WriteProperty(key, value);
            return this;
        }

        public static SettingsData ReadFromFile(SettingCategory category, string file)
        {
            var data = new SettingsData(category){
                Name = file
            };

            StringPropertyReader.ForFile(file).ReadProperties(data.WriteProperty);

            return data;
        }

        public override string ToString()
        {
            return string.Format("Settings Data, category {0}, provenance: {1}", Category, Name ?? "unknown");
        }

        public static IEnumerable<SettingsData> Order(IEnumerable<SettingsData> settings)
        {
            var list = new List<SettingsData>();

            list.AddRange(settings.Where(x => x.Category == SettingCategory.profile));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.environment));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.package));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.core));


            return list;
        }
    }
}