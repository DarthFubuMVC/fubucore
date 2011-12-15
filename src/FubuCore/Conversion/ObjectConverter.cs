using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    /*
     * TODO:

     * 2.) IObjectConverterFamily -- change the signature of the CreateConverter method?  Have it return an object?  Makes diagnostics better.

     * 4.) Make ObjectConverter depend on services?
     * 5.) ConversionRequest
     *      a.) Get<T>()
     *      b.) string Text

     * 7.) ServiceEnabledObjectConverter can do it for itself
     * 


     * 10.) Tweak up ServiceEnabledObjectConverter
     */

    public class ObjectConverter : IObjectConverter
    {
        public const string EMPTY = "EMPTY";
        public const string NULL = "NULL";
        public const string BLANK = "BLANK";



        private readonly Cache<Type, IConverterStrategy> _froms;
        private readonly IList<IObjectConverterFamily> _families = new List<IObjectConverterFamily>();

        public ObjectConverter()
        {
            _froms = new Cache<Type, IConverterStrategy>(createFinder);
            Clear();
        }

        private IConverterStrategy createFinder(Type type)
        {
            var family = _families.FirstOrDefault(x => x.Matches(type, this));
            if (family != null)
            {
                return family.CreateConverter(type, _froms);
            }

            throw new ArgumentException("No conversion exists for " + type.AssemblyQualifiedName);
        }

        public bool CanBeParsed(Type type)
        {
            return _froms.Has(type) || _families.Any(x => x.Matches(type, this));
        }

        public IConverterStrategy StrategyFor(Type type)
        {
            return _froms[type];
        }

        public void RegisterConverter<T>(Func<string, T> finder)
        {
            _froms[typeof(T)] = new LambdaConverterStrategy<T>(finder);
        }

        public void RegisterConverterFamily(IObjectConverterFamily family)
        {
            _families.Insert(0, family);
        }

        public void Clear()
        {
            _froms.ClearAll();
            RegisterConverter(parseString);
            RegisterConverter(DateTimeConverter.GetDateTime);
            RegisterConverter(DateTimeConverter.GetTimeSpan);
            RegisterConverter(TimeZoneInfo.FindSystemTimeZoneById);

            _families.Clear();
            _families.Add(new EnumConverterFamily());
            _families.Add(new ArrayConverterFamily());
            _families.Add(new NullableConverterFamily());
            _families.Add(new StringConstructorConverterFamily());
            _families.Add(new TypeDescripterConverterFamily());
        }

        protected virtual object getService(Type type)
        {
            throw new NotSupportedException();
        }

        public virtual object From(IConversionRequest request, Type type)
        {
            return request.Text == NULL ? null : _froms[type].Convert(request);
        }

        

        public virtual object FromString(string stringValue, Type type)
        {
            return From(new ConversionRequest(stringValue, getService), type);
        }

        public virtual T FromString<T>(string stringValue)
        {
            return (T)FromString(stringValue, typeof(T));
        }

        private static string parseString(string stringValue)
        {
            if (stringValue == BLANK || stringValue == EMPTY)
            {
                return string.Empty;
            }

            if (stringValue == NULL)
            {
                return null;
            }

            return stringValue;
        }


    }
}