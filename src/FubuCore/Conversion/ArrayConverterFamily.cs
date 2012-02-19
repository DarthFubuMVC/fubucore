using System;
using System.ComponentModel;
using FubuCore.Descriptions;

namespace FubuCore.Conversion
{
    [Description("Builds an array of a type from a comma delimited string, applying the conversion against each value for the element type")]
    public class ArrayConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, ConverterLibrary converter)
        {
            if (type.IsArray && converter.CanBeParsed(type.GetElementType())) return true;


            return (type.IsGenericEnumerable() && converter.CanBeParsed(type.GetGenericArguments()[0]));
        }

        public IConverterStrategy CreateConverter(Type type, Func<Type, IConverterStrategy> converterSource)
        {
            var innerType = type.IsGenericEnumerable() ? type.GetGenericArguments()[0] : type.GetElementType();

            var singleObjectFinder = converterSource(innerType);

            return new ArrayConverterStrategy(innerType, singleObjectFinder);
        }

        #region Nested type: ArrayConverterStrategy

        public class ArrayConverterStrategy : IConverterStrategy
        {
            private readonly IConverterStrategy _inner;
            private readonly Type _innerType;

            public ArrayConverterStrategy(Type innerType, IConverterStrategy inner)
            {
                _innerType = innerType;
                _inner = inner;
            }

            public object Convert(IConversionRequest request)
            {
                var stringValue = request.Text;
                if (stringValue.ToUpper() == StringConverterStrategy.EMPTY || stringValue.Trim().IsEmpty())
                {
                    return Array.CreateInstance(_innerType, 0);
                }

                var strings = stringValue.ToDelimitedArray();
                var array = Array.CreateInstance(_innerType, strings.Length);

                for (var i = 0; i < strings.Length; i++)
                {
                    var value = _inner.Convert(request.AnotherRequest(strings[i]));
                    array.SetValue(value, i);
                }

                return array;
            }

            public void Describe(Description description)
            {
                description.Title = "Array";
                description.ShortDescription = _innerType.FullName + "[]";
            }
        }

        #endregion
    }
}