using System;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Descriptions;
using FubuCore.Util;

namespace FubuCore.Binding
{
    [Description("Attempts to bind a property by finding a value matching the property name and converting the raw value to the property type")]
    public class ConversionPropertyBinder : IPropertyBinder, DescribesItself
    {
        private readonly Cache<PropertyInfo, ValueConverter> _cache = new Cache<PropertyInfo, ValueConverter>();
        private IValueConverterRegistry _converters;

        public ConversionPropertyBinder(IValueConverterRegistry converters)
        {
            _cache.OnMissing = prop => converters.FindConverter(prop);
            _converters = converters;
        }

        public bool Matches(PropertyInfo property)
        {
            return _cache[property] != null;
        }


        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.ForProperty(property, x =>
            {
                var data = x.RawValueFromRequest != null ? x.RawValueFromRequest.RawValue : null;
                if (data != null)
                {
                    var converter = _cache[property];

                    context.Logger.Chose(property, converter);

                    var value = converter.Convert(x);

                    x.SetPropertyValue(value);
                }
            });
        }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            return _cache[property];
        }

        public void Describe(Description description)
        {
            var list = description.AddList("ConversionFamilies", _converters.AllConverterFamilies());
            list.IsOrderDependent = true;
            list.Label = "Conversion Families";
        }
    }
}