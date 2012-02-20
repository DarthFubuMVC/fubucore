using System.ComponentModel;
using System.Reflection;

namespace FubuCore.Binding
{
    [Description("Passthrough of System.Object properties")]
    public class ObjectTypeFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType == typeof(object);
        }

        public override object Convert(IPropertyContext context)
        {
            if (context.RawValueFromRequest == null) return null;
            return context.RawValueFromRequest.RawValue;
        }
    }
}