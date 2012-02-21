using System;
using System.Reflection;

namespace FubuCore.Binding.InMemory
{
    public interface IBindingLogger
    {
        void Chose(Type modelType, IModelBinder binder);  // Starts a new one
        void Chose(PropertyInfo property, IPropertyBinder binder);
        void Chose(PropertyInfo property, ValueConverter converter);


        void PushElement(Type elementType);
        void FinishedModel();

        void UsedValue(BindingValue value);
    }
}