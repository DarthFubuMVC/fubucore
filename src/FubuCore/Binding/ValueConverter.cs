using System;
using FubuCore.Descriptions;

namespace FubuCore.Binding
{
    public interface ValueConverter
    {
        object Convert(IPropertyContext context);
    }

    public class LambdaValueConverter : ValueConverter, HasDescription
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

        public Description GetDescription()
        {
            return new Description(){
                Title = "Lambda",
                ShortDescription = _description
            };
        }
    }
}