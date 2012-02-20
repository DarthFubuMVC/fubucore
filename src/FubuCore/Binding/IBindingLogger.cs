using System;
using System.Reflection;

namespace FubuCore.Binding
{
    public interface IBindingLogger
    {
        void ChoseModelBinder(Type modelType, IModelBinder binder);
        void ChosePropertyBinder(PropertyInfo property, IPropertyBinder binder);
        void ChoseValueConverter(PropertyInfo property, ValueConverter converter);
    }
}