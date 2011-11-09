using System;
using System.Globalization;
using FubuCore;

namespace FubuLocalization.Basic
{
    public class LocalizationMissingHandler : ILocalizationMissingHandler
    {
        private readonly ILocalizationStorage _storage;
        private readonly CultureInfo _defaultCulture;

        public LocalizationMissingHandler(ILocalizationStorage storage, CultureInfo defaultCulture)
        {
            _storage = storage;
            _defaultCulture = defaultCulture;
        }

        public string FindMissingText(StringToken key, CultureInfo culture)
        {
            var defaultValue = culture.Name + "_" + key.Key;
            if (key.DefaultValue.IsNotEmpty() && culture.Equals(_defaultCulture))
            {
                defaultValue = key.DefaultValue;
            }

            _storage.WriteMissing(key.Key, defaultValue, culture);

            return defaultValue;
        }

        public string FindMissingProperty(PropertyToken property, CultureInfo culture)
        {
            var defaultValue = culture.Equals(_defaultCulture)
                                   ? property.Header ?? property.DefaultHeaderText(culture) ?? property.PropertyName
                                   : property.DefaultHeaderText(culture) ?? culture.Name + "_" + property.PropertyName;

            _storage.WriteMissing(property.StringTokenKey, defaultValue, culture);

            return defaultValue;
        }


    }
}