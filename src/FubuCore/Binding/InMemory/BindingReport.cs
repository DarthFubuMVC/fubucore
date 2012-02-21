using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;

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

        public PropertyBindingReport For<T>(Expression<Func<T, object>> expression)
        {
            var property = expression.ToAccessor().InnerProperty;
            return For(property);
        }
    }
}