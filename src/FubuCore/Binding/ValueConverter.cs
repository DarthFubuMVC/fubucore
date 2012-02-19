using System;
using FubuCore.Descriptions;

namespace FubuCore.Binding
{
    public interface ValueConverter
    {
        object Convert(IPropertyContext context);
    }

    public class LambdaValueConverter : ValueConverter, DescribesItself
    {
        private readonly Func<IPropertyContext, object> _converter;
        private readonly string _description;

        public LambdaValueConverter(Func<IPropertyContext, object> converter, string description)
        {
            _converter = converter;
            _description = description;
        }

        public object Convert(IPropertyContext context)
        {
            return _converter(context);
        }

        public void Describe(Description description)
        {
            description.Title = "Lambda";
            description.ShortDescription = _description;
        }
    }
}