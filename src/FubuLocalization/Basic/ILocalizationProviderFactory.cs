using System.Globalization;

namespace FubuLocalization.Basic
{
    public interface ILocalizationProviderFactory
    {
        void LoadAll();
        ILocalizationDataProvider BuildProvider(CultureInfo culture);
        void ApplyToLocalizationManager();
    }
}