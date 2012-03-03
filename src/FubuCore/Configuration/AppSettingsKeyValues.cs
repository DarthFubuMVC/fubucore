using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class AppSettingsKeyValues : IKeyValues
    {
        private readonly NameValueCollection _settings = ConfigurationManager.AppSettings;

        public bool Has(string key)
        {
            if (!_settings.HasKeys())
            {
                return false;
            }

            return _settings.AllKeys.Contains(key);
        }

        public string Get(string key)
        {
            return _settings[key];
        }

        public IEnumerable<string> GetKeys()
        {
            if (!_settings.HasKeys())
            {
                return Enumerable.Empty<string>();
            }

            return _settings.AllKeys;
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (!Has(key)) return false;

            callback(key, Get(key));

            return true;
        }
    }
}