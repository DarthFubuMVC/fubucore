using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FubuCore.Binding.InMemory
{
    public class BindingReport
    {
        private readonly IModelBinder _binder;
        private readonly Type _modelType;
        private readonly IList<PropertyBindingReport> _properties = new List<PropertyBindingReport>();

        public BindingReport(Type modelType, IModelBinder binder)
        {
            _modelType = modelType;
            _binder = binder;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public IList<PropertyBindingReport> Properties
        {
            get { return _properties; }
        }

        public IModelBinder Binder
        {
            get { return _binder; }
        }

        public PropertyBindingReport LastProperty
        {
            get { return _properties.Last(); }
        }

        public void AddProperty(PropertyInfo property, IPropertyBinder binder)
        {
            var report = new PropertyBindingReport(property, binder);
            _properties.Add(report);
        }

        public PropertyBindingReport For(PropertyInfo property)
        {
            return _properties.LastOrDefault(x => property.PropertyMatches(x.Property));
        }
    }

    public class PropertyBindingReport
    {
        private readonly IPropertyBinder _binder;
        private readonly IList<ElementBinding> _elements = new List<ElementBinding>();
        private readonly PropertyInfo _property;
        private readonly IList<BindingValue> _values = new List<BindingValue>();
        private ValueConverter _converter;
        private BindingReport _nested;

        public PropertyBindingReport(PropertyInfo property, IPropertyBinder binder)
        {
            _property = property;
            _binder = binder;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public IPropertyBinder Binder
        {
            get { return _binder; }
        }

        public ValueConverter Converter
        {
            get { return _converter; }
        }

        public void Chose(ValueConverter converter)
        {
            _converter = converter;
        }

        public void Used(BindingValue value)
        {
            _values.Add(value);
        }

        public BindingReport BindAsNestedChild(IModelBinder binder)
        {
            _nested = new BindingReport(_property.PropertyType, binder);
            return _nested;
        }

        public ElementBinding AddElement(Type elementType, IModelBinder binder)
        {
            var binding = new ElementBinding(_elements.Count, elementType, binder);
            _elements.Add(binding);

            return binding;
        }
    }

    public class ElementBinding : BindingReport
    {
        private readonly int _index;

        public ElementBinding(int index, Type elementType, IModelBinder binder) : base(elementType, binder)
        {
            _index = index;
        }

        public int Index
        {
            get { return _index; }
        }
    }
}