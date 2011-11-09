using System.Globalization;

namespace FubuLocalization
{
    public interface ILocalizationMissingHandler
    {
        string FindMissingText(StringToken key, CultureInfo culture);
        string FindMissingProperty(PropertyToken property, CultureInfo culture);
    }
}