using System;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Util;

namespace FubuCore.Binding
{
    [Description("Attempts to bind a property by finding a value matching the property name and converting the raw value to the property type")]
    public class ConversionPropertyBinder : IPropertyBinder
    {
        private readonly Cache<PropertyInfo, ValueConverter> _cache = new Cache<PropertyInfo, ValueConverter>();

        public ConversionPropertyBinder(IValueConverterRegistry converters)
        {
            _cache.OnMissing = prop => converters.FindConverter(prop);
        }

        public bool Matches(PropertyInfo property)
        {
            return _cache[property] != null;
        }


        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.ForProperty(property, x =>
            {
                var converter = _cache[property];

                context.Logger.ChoseValueConverter(property, converter);

                var value = converter.Convert(x);
                    
                x.SetPropertyValue(value);
            });
        }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            return _cache[property];
        }
    }
}