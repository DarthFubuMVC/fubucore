using System;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public interface IObjectConverterFamily
    {
        // ObjectConverter calls this method on unknown types
        // to ask an IObjectConverterFamily if it "knows" how
        // to convert a string into the given type
        bool Matches(Type type, IObjectConverter converter);

        // If Matches() returns true for a given type, 
        // ObjectConverter asks this IObjectConverterFamily
        // for a converter Lambda and calls its 
        // RegisterFinder() method behind the scenes to cache
        // the Lambda for later usage
        Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters);
    }
}