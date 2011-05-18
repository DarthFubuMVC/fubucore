using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FubuCore.Configuration
{
    public static class XmlSettingsParser
    {
        public static SettingsData Parse(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            var data = Parse(document.DocumentElement);
            data.Provenance = file;

            return data;
        }

        public static SettingsData Parse(XmlElement element)
        {
            var category = (SettingCategory)(element.HasAttribute("category")
                                               ? Enum.Parse(typeof(SettingCategory), element.GetAttribute("category"))
                                               : SettingCategory.core);

            var data = new SettingsData(category);

            element.SelectNodes("add").OfType<XmlElement>().Each(elem =>
            {
                var key = elem.GetAttribute("key");
                var value = elem.GetAttribute("value");
                data[key] = value;
            });

            return data;
        }


    }
}