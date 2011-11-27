using System;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class NullableConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, IObjectConverter converter)
        {
            return type.IsNullable();
        }

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            Func<string, object> inner = converters[type.GetInnerTypeFromNullable()];

            return stringValue =>
            {
                if (stringValue == ObjectConverter.NULL || stringValue == null) return null;
                if (stringValue == string.Empty && type != typeof(string)) return null;

                return inner(stringValue);
            };
        }
    }
}