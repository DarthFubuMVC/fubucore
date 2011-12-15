using System;

namespace FubuCore.Conversion
{
    public class ConversionRequest : IConversionRequest
    {
        private readonly Func<Type, object> _finder;
        private readonly string _text;

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
            return (T) _finder(typeof (T));
        }
    }
}