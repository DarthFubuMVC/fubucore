using System;
using FubuCore.Binding;


namespace FubuCore.Configuration
{
    public class AppSettingsProvider : ISettingsProvider
    {
        private readonly IObjectResolver _resolver;

        public AppSettingsProvider(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T SettingsFor<T>() where T : class, new()
        {
            Type settingsType = typeof (T);

            object value = SettingsFor(settingsType);

            return (T) value;
        }

        public object SettingsFor(Type settingsType)
        {
            var result = _resolver.BindModel(settingsType, new AppSettingsRequestData(settingsType));

            result.AssertNoProblems(settingsType);

            return result.Value;
        }


    }
}