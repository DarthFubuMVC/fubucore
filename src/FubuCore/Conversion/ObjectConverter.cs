using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore.Conversion
{
    /*
     * TODO:

     * 2.) IObjectConverterFamily -- change the signature of the CreateConverter method?  Have it return an object?  Makes diagnostics better.

     */




    public interface IObjectConverter
    {
        /// <summary>
        /// Given a string and a .Net type, read this string
        /// and give me back a corresponding instance of that
        /// type
        /// </summary>
        /// <param name="stringValue">The value to convert</param>
        /// <param name="type">The desired destination type</param>
        /// <returns>The value converted into the specified desination type</returns>
        object FromString(string stringValue, Type type);
        /// <summary>
        /// Given a string and a .Net type, T, read this string
        /// and give me back a corresponding instance of type T.
        /// </summary>
        /// <typeparam name="T">The desired destination type</typeparam>
        /// <param name="stringValue">The value to convert</param>
        /// <returns>The value converted into the specified desination type</returns>
        T FromString<T>(string stringValue);
        /// <summary>
        /// Determines whether there is conversion support registered for the specified type
        /// </summary>
        /// <param name="type">The desired destination type</param>
        /// <returns>True if conversion to this type is supported, otherwise False.</returns>
        bool CanBeParsed(Type type);



        /// <summary>
        /// Returns an IConverterStrategy that "knows" how to convert
        /// to a particular type from a string. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IConverterStrategy StrategyFor(Type type);

        object From(IConversionRequest request, Type type);
    }


    public class ConverterLibrary
    {
        private readonly Cache<Type, IConverterStrategy> _froms;
        private readonly IList<IObjectConverterFamily> _families = new List<IObjectConverterFamily>();

        public ConverterLibrary() : this(new IObjectConverterFamily[0])
        {
        }

        public ConverterLibrary(IEnumerable<IObjectConverterFamily> families)
        {
            _froms = new Cache<Type, IConverterStrategy>(createFinder);

            // Strategies that are injected *must* be put first
            _families.AddRange(families);

            _families.Add(new StringConverterStrategy());
            _families.Add(new DateTimeConverter());
            _families.Add(new TimeSpanConverter());
            _families.Add(new TimeZoneConverter());

            _families.Add(new EnumConverterFamily());
            _families.Add(new ArrayConverterFamily());
            _families.Add(new NullableConverterFamily());
            _families.Add(new StringConstructorConverterFamily());
            _families.Add(new TypeDescripterConverterFamily());
        }

        public void RegisterConverter<T>(Func<string, T> finder)
        {
            _froms[typeof(T)] = new LambdaConverterStrategy<T>(finder);
        }

        public void RegisterConverter<TReturnType, TService>(Func<TService, string, TReturnType> converter)
        {
            _froms[typeof (TReturnType)] = new LambdaConverterStrategy<TReturnType, TService>(converter);
        }

        public void RegisterConverterFamily(IObjectConverterFamily family)
        {
            _families.Insert(0, family);
        }

        private IConverterStrategy createFinder(Type type)
        {
            var family = _families.FirstOrDefault(x => x.Matches(type, this));
            if (family != null)
            {
                return family.CreateConverter(type, t => _froms[t]);
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
    }


    public class ObjectConverter : IObjectConverter
    {
        private readonly ConverterLibrary _library;
        private readonly Func<Type, object> _finder;

        public const string NULL = "NULL";


        public ObjectConverter() : this(type => { throw new NotSupportedException(); }, new ConverterLibrary(new IObjectConverterFamily[0]))
        {
        }

        public ObjectConverter(IServiceLocator services, ConverterLibrary library)
            : this(type => services.GetInstance(type), library)
        {
        }

        private ObjectConverter(Func<Type, object> finder, ConverterLibrary library)
        {
            _library = library;
            _finder = finder;
        }

        public bool CanBeParsed(Type type)
        {
            return _library.CanBeParsed(type);
        }

        public IConverterStrategy StrategyFor(Type type)
        {
            return _library.StrategyFor(type);
        }

        public virtual object From(IConversionRequest request, Type type)
        {
            return request.Text == NULL ? null : StrategyFor(type).Convert(request);
        }

        public virtual object FromString(string stringValue, Type type)
        {
            return From(new ConversionRequest(stringValue, _finder), type);
        }

        public virtual T FromString<T>(string stringValue)
        {
            return (T)FromString(stringValue, typeof(T));
        }



    }
}