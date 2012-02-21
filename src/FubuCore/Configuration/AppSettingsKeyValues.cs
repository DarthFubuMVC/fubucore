using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class AppSettingsKeyValues : IKeyValues
    {
        public bool ContainsKey(string key)
        {
            if (!ConfigurationManager.AppSettings.HasKeys())
            {
                return false;
            }

            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
        }

        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public IEnumerable<string> GetKeys()
        {
            if (!ConfigurationManager.AppSettings.HasKeys())
            {
                return Enumerable.Empty<string>();
            }

            return ConfigurationManager.AppSettings.AllKeys;
        }
    }
}