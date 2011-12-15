using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class ConverterLibrary
    {
        private readonly IList<IObjectConverterFamily> _families = new List<IObjectConverterFamily>();
        private readonly Cache<Type, IConverterStrategy> _froms;

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
            _froms[typeof (T)] = new LambdaConverterStrategy<T>(finder);
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
}