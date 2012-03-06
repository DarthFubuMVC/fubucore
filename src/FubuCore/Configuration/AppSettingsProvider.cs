using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Reflection;

namespace FubuCore.Configuration
{
    public class AppSettingsProvider : ISettingsProvider
    {
        private readonly IObjectResolver _resolver;
        private readonly Lazy<IValueSource> _values;

        public AppSettingsProvider(IObjectResolver resolver)
        {
            _resolver = resolver;

            _values = new Lazy<IValueSource>(() => SettingsData.For(new AppSettingsKeyValues()));
        }

        public T SettingsFor<T>() where T : class, new()
        {
            var settingsType = typeof (T);

            var value = SettingsFor(settingsType);

            return (T) value;
        }

        public object SettingsFor(Type settingsType)
        {
            var requestData = new RequestData(_values.Value.GetChild(settingsType.Name));

            var result = _resolver.BindModel(settingsType, requestData);

            result.AssertNoProblems(settingsType);

            return result.Value;
        }

        public static string KeyFor<T>(Expression<Func<T, object>> property)
        {
            return typeof (T).Name + "." + property.ToAccessor().Name;
        }

        public static string GetValueFor<T>(Expression<Func<T, object>> property)
        {
            var key = KeyFor(property);
            return (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                       ? ConfigurationManager.AppSettings[key]
                       : string.Empty;
        }
    }
}