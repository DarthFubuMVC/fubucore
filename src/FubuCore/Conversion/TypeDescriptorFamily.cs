using System;
using System.ComponentModel;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class TypeDescriptorFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, IObjectConverter converter)
        {
            return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof (string));
        }

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return s => converter.ConvertFromString(s);
        }
    }
}