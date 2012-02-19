using System;
using System.ComponentModel;
using FubuCore.Descriptions;

namespace FubuCore.Conversion
{
    [Description("Uses Enum.Parse() to handle conversion to enumeration types")]
    public class EnumConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, ConverterLibrary converter)
        {
            return type.IsEnum;
        }

        public IConverterStrategy CreateConverter(Type type, Func<Type, IConverterStrategy> converterSource)
        {
            return new EnumConversionStrategy(type);
        }

        #region Nested type: EnumConversionStrategy

        public class EnumConversionStrategy : IConverterStrategy, DescribesItself
        {
            private readonly Type _enumType;

            public EnumConversionStrategy(Type enumType)
            {
                _enumType = enumType;
            }

            public void Describe(Description description)
            {
                description.Title = "Enum.Parse";
                description.ShortDescription = "Enum.Parse(typeof(" + _enumType.FullName + "), text)";
            }

            public object Convert(IConversionRequest request)
            {
                return Enum.Parse(_enumType, request.Text, true);
            }
        }

        #endregion
    }
}