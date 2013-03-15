using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FubuCore.Binding
{
    [Description("Converts to booean values, HTML checkbox friendly conversion")]
    public class BooleanFamily : StatelessConverter
    {
        private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(bool));
        private static readonly IList<string> _positives = new List<string> { "yes", "y" };
        private static readonly IList<string> _negatives = new List<string> { "no", "n" };
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

            if (valueString.IsEmpty()) return false;
            if (valueString.Contains(context.Property.Name)) return true;
            if (valueString.EqualsIgnoreCase(CheckboxOn)) return true;
            if (_positives.Any(x => x.Equals(valueString, StringComparison.OrdinalIgnoreCase))) return true;
            if (_negatives.Any(x => x.Equals(valueString, StringComparison.OrdinalIgnoreCase))) return false;

            return (bool)_converter.ConvertFrom(rawValue);
        }
    }
}