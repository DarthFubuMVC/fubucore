using System.Reflection;

namespace FubuCore.Binding
{
    public class ObjectTypeFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType == typeof(object);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.PropertyValue;
        }
    }
}