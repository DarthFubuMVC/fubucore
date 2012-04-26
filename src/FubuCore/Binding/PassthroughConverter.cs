using System.Reflection;
using FubuCore.Descriptions;

namespace FubuCore.Binding
{
    public class PassthroughConverter<T> : StatelessConverter, DescribesItself
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsAssignableFrom(typeof(T)) && property.PropertyType != typeof(object);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.RawValueFromRequest != null ? context.RawValueFromRequest.RawValue : null;
        }

        public void Describe(Description description)
        {
            description.Title = "Pass thru";
            description.ShortDescription = "Pass through conversion of " + typeof (T).FullName;
        }
    }
}