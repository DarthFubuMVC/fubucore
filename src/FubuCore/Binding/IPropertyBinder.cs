using System;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BindingAttribute : Attribute
    {
        public abstract void Bind(PropertyInfo property, IBindingContext context);
    }

    public class AttributePropertyBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<BindingAttribute>();
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            property.ForAttribute<BindingAttribute>(att => att.Bind(property, context));
        }
    }

    public interface IPropertyBinder
    {
        bool Matches(PropertyInfo property);
        void Bind(PropertyInfo property, IBindingContext context);
    }

    public class IgnorePropertyBinder : IPropertyBinder
    {
        private readonly Func<PropertyInfo, bool> _filter;

        public IgnorePropertyBinder(Func<PropertyInfo, bool> filter)
        {
            _filter = filter;
        }

        public bool Matches(PropertyInfo property)
        {
            return _filter(property);
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            // no-op
        }
    }
}