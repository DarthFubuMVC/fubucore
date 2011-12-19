using System;
using System.ComponentModel;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    /// <summary>
    /// Uses the built in TypeDescriptor in .Net to convert objects from strings
    /// </summary>
    public class TypeDescripterConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, ConverterLibrary converter)
        {
            try
            {
                return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IConverterStrategy CreateConverter(Type type, Cache<Type, IConverterStrategy> converters)
        {
            return new TypeDescriptorConversionStrategy(type);
        }

        public class TypeDescriptorConversionStrategy : IConverterStrategy
        {
            private readonly TypeConverter _converter;
            private Type _type; // Captured for diagnostics

            public TypeDescriptorConversionStrategy(Type type)
            {
                _converter = TypeDescriptor.GetConverter(type);
                _type = type;
            }

            public object Convert(IConversionRequest request)
            {
                return _converter.ConvertFromString(request.Text);
            }
        }
    }
}