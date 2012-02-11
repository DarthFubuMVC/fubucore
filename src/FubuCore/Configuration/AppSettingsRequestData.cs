using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using FubuCore.Binding;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.Configuration
{
    public class AppSettingsRequestData : RequestDataBase
    {
        protected override object fetch(string key)
        {
            return ConfigurationManager.AppSettings[key];
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

        public override IEnumerable<string> GetKeys()
        {
            return ConfigurationManager.AppSettings.AllKeys;
        }

        protected override string source
        {
            get { return "AppSettings"; }
        }
    }
}