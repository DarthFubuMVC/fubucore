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
        public bool Matches(Type type, IObjectConverter converter)
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

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            var converter = TypeDescriptor.GetConverter(type);

            return text => converter.ConvertFrom(text);
        }
    }
}