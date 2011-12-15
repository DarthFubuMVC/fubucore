namespace FubuLocalization
{
    public static class LocalizationExtensions
    {
        public static LocalizationKey ToLocalizationKey(this PropertyToken propertyInfo)
        {
            return new LocalizationKey(propertyInfo.PropertyName, propertyInfo.ParentTypeName);
        }

    }
}