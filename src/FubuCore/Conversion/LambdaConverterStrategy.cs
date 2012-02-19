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

        public void Describe(Description description)
        {
            description.Title = "Lambda";
            description.ShortDescription = _description;
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

        public void Describe(Description description)
        {
            description.Title = "Lambda:" + typeof (TService).Name;
            description.ShortDescription = _description;
        }
    }
}