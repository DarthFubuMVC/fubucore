using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using FubuCore.Binding;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.Configuration
{
    public class AppSettingsRequestData : IRequestData
    {
        private static readonly KeyValueConfigurationCollection Settings;
        static AppSettingsRequestData()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Settings = config.AppSettings.Settings;
        }

        public object Value(string key)
        {
            return Settings[key].Value;
        }

        public bool Value(string key, Action<object> callback)
        {
            if (Settings.AllKeys.Contains(key))
            {
                callback(Value(key));
                return true;
            }

            return false;
        }

        public bool HasAnyValuePrefixedWith(string key)
        {
            return Settings.AllKeys.Any(x => x.StartsWith(key));
        }

        public static string KeyFor<T>(Expression<Func<T, object>> property)
        {
            return typeof(T).Name + "." + property.ToAccessor().Name;
        }

        public static string GetValueFor<T>(Expression<Func<T, object>> property)
        {
            var key = KeyFor(property);
            return (Settings.AllKeys.Contains(key)) ? Settings[key].Value : string.Empty;
        }

        public IEnumerable<string> GetKeys()
        {
            return Settings.AllKeys;
        }
    }
}