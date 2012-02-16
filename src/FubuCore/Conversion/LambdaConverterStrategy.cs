using System;
using FubuCore.Descriptions;

namespace FubuCore.Conversion
{
    public class LambdaConverterStrategy<T> : IConverterStrategy
    {
        private readonly Func<string, T> _finder;
        private readonly string _description;

        public LambdaConverterStrategy(Func<string, T> finder, string description)
        {
            _finder = finder;
            _description = description;
        }

        public object Convert(IConversionRequest request)
        {
            return _finder(request.Text);
        }

        public Description GetDescription()
        {
            return new Description{
                Title = "Lambda",
                ShortDescription = _description
            };
        }
    }


    public class LambdaConverterStrategy<TReturnType, TService> : IConverterStrategy
    {
        private readonly Func<TService, string, TReturnType> _finder;
        private readonly string _description;

        public LambdaConverterStrategy(Func<TService, string, TReturnType> finder, string description)
        {
            _finder = finder;
            _description = description;
        }

        public object Convert(IConversionRequest request)
        {
            return _finder(request.Get<TService>(), request.Text);
        }

        public Description GetDescription()
        {
            return new Description{
                Title = "Lambda:" + typeof(TService).Name,
                ShortDescription = _description
            };
        }
    }
}