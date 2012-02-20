using System;
using System.ComponentModel;
using System.Reflection;

namespace FubuCore.Binding
{
    [Description("Converts to booean values, HTML checkbox friendly conversion")]
    public class BooleanFamily : StatelessConverter
    {
        private static TypeConverter _converter = TypeDescriptor.GetConverter(typeof(bool));
        public const string CheckboxOn = "on";

        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsTypeOrNullableOf<bool>();
        }

        public override object Convert(IPropertyContext context)
        {
            var rawValue = context.RawValueFromRequest.RawValue;

            if (rawValue is bool) return rawValue;

            var valueString = rawValue.ToString();

            return valueString.Contains(context.Property.Name)
            || valueString.EqualsIgnoreCase(CheckboxOn)
            || (bool)_converter.ConvertFrom(rawValue);
        }
    }
}