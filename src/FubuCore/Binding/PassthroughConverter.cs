using System;
using System.Reflection;
using FubuCore.Descriptions;

namespace FubuCore.Binding
{
    public class PassthroughConverter<T> : StatelessConverter, HasDescription
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsAssignableFrom(typeof(T)) && property.PropertyType != typeof(object);
        }

        public override object Convert(IPropertyContext context)
        {
            return context.PropertyValue;
        }

        public Description GetDescription()
        {
            return new Description{
                Title = "Pass thru",
                ShortDescription = "Pass through conversion of " + typeof(T).FullName
            };
        }
    }
}