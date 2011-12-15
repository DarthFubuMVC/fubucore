using System;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public interface IObjectConverterFamily
    {
        // ObjectConverter calls this method on unknown types
        // to ask an IObjectConverterFamily if it "knows" how
        // to convert a string into the given type
        bool Matches(Type type, ConverterLibrary converter);

        // If Matches() returns true for a given type, 
        // ObjectConverter asks this IObjectConverterFamily
        // for a converter Lambda and calls its 
        // RegisterFinder() method behind the scenes to cache
        // the Lambda for later usage
        IConverterStrategy CreateConverter(Type type, Cache<Type, IConverterStrategy> converters);
    }


    public abstract class StatelessConverter : IObjectConverterFamily, IConverterStrategy
    {
        public abstract bool Matches(Type type, ConverterLibrary converter);

        public IConverterStrategy CreateConverter(Type type, Cache<Type, IConverterStrategy> converters)
        {
            return this;
        }

        public abstract object Convert(IConversionRequest request);
    }

    public abstract class StatelessConverter<TReturnType> : StatelessConverter
    {
        public sealed override bool Matches(Type type, ConverterLibrary converter)
        {
            return type == typeof (TReturnType);
        }

        public sealed override object Convert(IConversionRequest request)
        {
            return convert(request.Text);
        }

        protected abstract TReturnType convert(string text);
    }

    

    public interface IConversionRequest
    {
        T Get<T>();
        string Text { get;}

        IConversionRequest AnotherRequest(string text);
    }

    public class ConversionRequest : IConversionRequest
    {
        private readonly string _text;
        private readonly Func<Type, object> _finder;

        public ConversionRequest(string text)
            : this(text, type => { throw new NotSupportedException(); })
        {
            _text = text;
        }

        public ConversionRequest(string text, Func<Type, object> finder)
        {
            _text = text;
            _finder = finder;
        }

        public string Text
        {
            get { return _text; }
        }

        public IConversionRequest AnotherRequest(string text)
        {
            return new ConversionRequest(text, _finder);
        }

        public T Get<T>()
        {
            return (T) _finder(typeof(T));
        }
    }

    public interface IConverterStrategy
    {
        object Convert(IConversionRequest request);
    }

    public static class ConverterStrategyExtensions
    {
        public static object Convert(this IConverterStrategy strategy, string text)
        {
            return strategy.Convert(new ConversionRequest(text));
        }
    }

    // TODO -- come back here and add the object descriptor stuff
    // for diagnostics
    public class LambdaConverterStrategy<T> : IConverterStrategy
    {
        private readonly Func<string, T> _finder;

        public LambdaConverterStrategy(Func<string, T> finder)
        {
            _finder = finder;
        }

        public object Convert(IConversionRequest request)
        {
            return _finder(request.Text);
        }
    }


    // TODO -- come back here and add the object descriptor stuff
    // for diagnostics
    public class LambdaConverterStrategy<TReturnType, TService> : IConverterStrategy
    {
        private readonly Func<TService, string, TReturnType> _finder;

        public LambdaConverterStrategy(Func<TService, string, TReturnType> finder)
        {
            _finder = finder;
        }

        public object Convert(IConversionRequest request)
        {
            return _finder(request.Get<TService>(), request.Text);
        }
    }
    
}