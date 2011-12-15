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

        public IConverterStrategy CreateConverter(Type type, Cache<Type, IConverterStrategy> converters)
        {
            return new EnumConversionStrategy(type);
        }

        public class EnumConversionStrategy : IConverterStrategy
        {
            private readonly Type _enumType;

            public EnumConversionStrategy(Type enumType)
            {
                _enumType = enumType;
            }

            public object Convert(IConversionRequest request)
            {
                return Enum.Parse(_enumType, request.Text, true);
            }
        }
    }
}