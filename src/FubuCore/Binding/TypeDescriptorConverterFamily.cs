using System;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class TypeDescriptorConverterFamily : IConverterFamily
    {
        private readonly Cache<Type, ValueConverter> _converters
            = new Cache<Type, ValueConverter>(type => new BasicValueConverter(type));

        public bool Matches(PropertyInfo property)
        {
            try
            {
                return TypeDescriptor.GetConverter(property.PropertyType).CanConvertFrom(typeof (string));
            }
            catch (Exception)
            {
                return false;
            }
        }


        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            return _converters[propertyType];
        }

        #region Nested type: BasicValueConverter

        public class BasicValueConverter : ValueConverter
        {
            private readonly TypeConverter _converter;

            public BasicValueConverter(Type propertyType)
            {
                _converter = TypeDescriptor.GetConverter(propertyType);
            }

            public object Convert(IPropertyContext context)
            {
                var propertyType = context.Property.PropertyType;

                if (context.PropertyValue != null)
                {
                    if (context.PropertyValue.GetType() == propertyType)
                    {
                        return context.PropertyValue;
                    }
                }

                return _converter.ConvertFrom(context.PropertyValue);
            }
        }

        #endregion
    }
}