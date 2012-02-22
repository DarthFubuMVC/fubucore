using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace FubuCore.Binding
{
    [Description("Recursively binds properties of a child property for supporting deep object graphs")]
    public class NestedObjectPropertyBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            return true;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            if (!context.HasChildRequest(property.Name))
            {
                return;
            }

            var childRequest = context.GetSubRequest(property.Name);
            context.BindObject(childRequest, property.PropertyType, o =>
            {
                property.SetValue(context.Object, o, null);
            });
        }
    }
}