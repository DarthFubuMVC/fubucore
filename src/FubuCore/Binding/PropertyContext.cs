using System;
using System.Reflection;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;

namespace FubuCore.Binding
{
    public class PropertyContext : IPropertyContext
    {
        private readonly IBindingContext _parent;
        private readonly IServiceLocator _services;
        private readonly PropertyInfo _property;
        private Lazy<BindingValue> _value;

        public PropertyContext(IBindingContext parent, IServiceLocator services, PropertyInfo property)
        {
            _parent = parent;
            _services = services;
            _property = property;

            _value = new Lazy<BindingValue>(() => _parent.Data.RawValue(Property.Name));
        }

        string IConversionRequest.Text
        {
            get { return RawValueFromRequest.RawValue as string; }
        }

        T IConversionRequest.Get<T>()
        {
            return _parent.Service<T>();
        }

        IConversionRequest IConversionRequest.AnotherRequest(string text)
        {
            return new ConversionRequest(text, Service);
        }

        public BindingValue RawValueFromRequest
        {
            get { return _value.Value; }
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public object Object
        {
            get { return _parent.Object; }
        }

        public T Service<T>()
        {
            return _parent.Service<T>();
        }

        public object Service(Type typeToFind)
        {
            return _services.GetInstance(typeToFind);
        }

        T IPropertyContext.ValueAs<T>()
        {
            return _parent.Data.ValueAs<T>(_property.Name);
        }

        bool IPropertyContext.ValueAs<T>(Action<T> continuation)
        {
            return _parent.Data.ValueAs(_property.Name, continuation);
        }

        public IBindingLogger Logger
        {
            get { return _parent.Logger; }
        }

        public IContextValues Data
        {
            get { return _parent.Data; }
        }

        public void SetPropertyValue(object value)
        {
            Property.SetValue(Object, value, null);
        }

        public object GetPropertyValue()
        {
            return Property.GetValue(Object, null);
        }
    }
}