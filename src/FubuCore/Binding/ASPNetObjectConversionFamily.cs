using System.Reflection;

namespace FubuCore.Binding
{
    public class ASPNetObjectConversionFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return AspNetAggregateDictionary.IsSystemProperty(property);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.PropertyValue;
        }
    }
}