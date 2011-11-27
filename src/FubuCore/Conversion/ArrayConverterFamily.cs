using System;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class ArrayConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, IObjectConverter converter)
        {
            if (type.IsArray && converter.CanBeParsed(type.GetElementType())) return true;




            return (type.IsGenericEnumerable() && converter.CanBeParsed(type.GetGenericArguments()[0]));
        }

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            var innerType = type.IsGenericEnumerable() ? type.GetGenericArguments()[0] : type.GetElementType();

            var singleObjectFinder = converters[innerType];

            return stringValue =>
            {
                if (stringValue.ToUpper() == ObjectConverter.EMPTY)
                {
                    return Array.CreateInstance(innerType, 0);
                }

                var strings = stringValue.ToDelimitedArray();
                var array = Array.CreateInstance(innerType, strings.Length);

                for (var i = 0; i < strings.Length; i++)
                {
                    var value = singleObjectFinder(strings[i]);
                    array.SetValue(value, i);
                }

                return array;
            };
        }
    }
}