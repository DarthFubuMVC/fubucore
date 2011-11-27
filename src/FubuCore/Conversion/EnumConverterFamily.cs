using System;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class EnumConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, IObjectConverter converter)
        {
            return type.IsEnum;
        }

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            return stringValue => Enum.Parse(type, stringValue, true);
        }
    }
}