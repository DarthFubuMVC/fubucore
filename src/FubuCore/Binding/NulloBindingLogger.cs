using System;
using System.Reflection;

namespace FubuCore.Binding
{
    public class NulloBindingLogger : IBindingLogger
    {
        public void ChoseModelBinder(Type modelType, IModelBinder binder)
        {
        }

        public void ChosePropertyBinder(PropertyInfo property, IPropertyBinder binder)
        {
        }

        public void ChoseValueConverter(PropertyInfo property, ValueConverter converter)
        {
        }
    }
}