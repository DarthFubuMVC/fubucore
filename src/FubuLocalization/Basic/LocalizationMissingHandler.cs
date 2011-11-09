using System;
using System.Globalization;

namespace FubuLocalization.Basic
{
    public class LocalizationMissingHandler : ILocalizationMissingHandler
    {
        private readonly ILocalizationStorage _storage;

        public LocalizationMissingHandler(ILocalizationStorage storage)
        {
            _storage = storage;
        }

        public string FindMissingText(StringToken key, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string FindMissingProperty(PropertyToken key, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string FindMissingText(string key, string defaultValue, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}