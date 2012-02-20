using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    public abstract class PropertyBinderTester
    {
        protected InMemoryBindingContext context;
        protected IPropertyBinder propertyBinder;

        protected bool matches(Expression<Func<AddressViewModel, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            return propertyBinder.Matches(property);
        }

        protected void shouldMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeTrue();
        }

        protected void shouldNotMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeFalse();
        }
    }

    public class AddressViewModel
    {
        public string Description { get; set; }
        public Address Address { get; set; }
        public bool ShouldShow { get; set; }
        public IList<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityViewModel
    {
        public LocalityViewModel()
        {
        }

        public LocalityViewModel(string zipCode, string countyName)
        {
            ZipCode = zipCode;
            CountyName = countyName;
        }

        public string ZipCode { get; set; }
        public string CountyName { get; set; }

        public bool Equals(LocalityViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ZipCode, ZipCode) && Equals(other.CountyName, CountyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LocalityViewModel)) return false;
            return Equals((LocalityViewModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ZipCode != null ? ZipCode.GetHashCode() : 0)*397) ^ (CountyName != null ? CountyName.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("ZipCode: {0}, CountyName: {1}", ZipCode, CountyName);
        }
    }
}