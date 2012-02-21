using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.Binding.Logging
{
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

        public IEnumerable<BindingValue> Values
        {
            get { return _values; }
        }

        public void Chose(ValueConverter converter)
        {
            _converter = converter;
        }

        public void Used(BindingValue value)
        {
            _values.Add(value);
        }

        public BindingReport Nested
        {
            get { return _nested; }
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

        public IList<ElementBinding> Elements
        {
            get { return _elements; }
        }

        public void AcceptVisitor(IBindingReportVisitor visitor)
        {
            visitor.Property(this);

            if (Nested != null)
            {
                Nested.AcceptVisitor(visitor);
            }

            Elements.Each(elem => elem.AcceptVisitor(visitor));

            visitor.EndProperty();
        }
    }
}