using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using FubuCore.Binding;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.Configuration
{
    public class AppSettingsRequestData : InMemoryRequestData
    {
        public AppSettingsRequestData(Type settingsType)
        {
            var values = new AppSettingsKeyValues();
            var prefix = settingsType.Name + ".";
            values.GetKeys().Where(x => x.StartsWith(prefix)).Each(key =>
            {
                var propKey = key.Split('.').Skip(1).Join("");

                this[propKey] = values.Get(key);
            });
        }

        public static string KeyFor<T>(Expression<Func<T, object>> property)
        {
            return typeof(T).Name + "." + property.ToAccessor().Name;
        }

        public static string GetValueFor<T>(Expression<Func<T, object>> property)
        {
            var key = KeyFor(property);
            return (ConfigurationManager.AppSettings.AllKeys.Contains(key)) ? ConfigurationManager.AppSettings[key] : string.Empty;
        }

    }
}