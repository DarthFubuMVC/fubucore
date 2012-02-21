using System;
using System.Reflection;

namespace FubuCore.Binding.InMemory
{
    public class NulloBindingLogger : IBindingLogger
    {
        public void Chose(Type modelType, IModelBinder binder)
        {
        }

        public void Chose(PropertyInfo property, IPropertyBinder binder)
        {
        }

        public void Chose(PropertyInfo property, ValueConverter converter)
        {
        }

        public void PushModel(Type modelType)
        {
            
        }

        public void PopModel()
        {
        }

        public void PushProperty(PropertyInfo property)
        {
        }

        public void PushElement(Type elementType)
        {
        }

        public void FinishedModel()
        {
        }

        public void UsedValue(BindingValue value)
        {
        }
    }
}