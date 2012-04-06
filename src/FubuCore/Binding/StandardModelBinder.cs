using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FubuCore.Descriptions;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    public interface IPropertySetter
    {
        void BindProperties(Type type, object instance, IBindingContext context);
    }

    [Description("The standard model binding using no-arg constructors, property binding policies, and value conversion policies")]
    public class StandardModelBinder : IModelBinder, DescribesItself, IPropertySetter
    {
        private readonly IPropertyBinderCache _propertyBinders;
        private readonly ITypeDescriptorCache _typeCache;

        public StandardModelBinder(BindingRegistry propertyBinders, ITypeDescriptorCache typeCache)
        {
            _propertyBinders = propertyBinders;
            _typeCache = typeCache;
        }

        public bool Matches(Type type)
        {
            return type.GetConstructors().Count(x => x.GetParameters().Length == 0) == 1;
        }

        public object Bind(Type type, IBindingContext context)
        {
            var instance = Activator.CreateInstance(type);
            BindProperties(type, instance, context);

            return instance;
        }

        public void BindProperties(Type type, object instance, IBindingContext context)
        {
            context.ForObject(instance, () => populate(type, context));
        }

        // Only exists for easier testing
        public void Populate(object target, IBindingContext context)
        {
            context.ForObject(target, () => populate(target.GetType(), context));
        }

        private void populate(Type type, IBindingContext context)
        {
            _typeCache.ForEachProperty(type, prop => PopulateProperty(type, prop, context));
        }

        public void PopulateProperty(Type type, PropertyInfo property, IBindingContext context)
        {
            var propertyBinder = _propertyBinders.BinderFor(property);
            PopulatePropertyWithBinder(property, context, propertyBinder);
        }

        public static void PopulatePropertyWithBinder(PropertyInfo property, IBindingContext context,
                                                      IPropertyBinder propertyBinder)
        {
            context.Logger.Chose(property, propertyBinder);
            context.ForProperty(property, propertyContext =>
            {
                propertyBinder.Bind(property, context);
            });
        }

        public void Describe(Description description)
        {
            var list = description.AddList("Property Binders", _propertyBinders.AllPropertyBinders());
            list.Label = "Property Binders";
            list.IsOrderDependent = true;
        }
    }
}