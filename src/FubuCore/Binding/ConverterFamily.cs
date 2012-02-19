using System;
using System.Reflection;
using FubuCore.Descriptions;

namespace FubuCore.Binding
{
    public class ConverterFamily : IConverterFamily, DescribesItself
    {
        private readonly Func<IValueConverterRegistry, PropertyInfo, ValueConverter> _builder;
        private readonly string _description;
        private readonly Predicate<PropertyInfo> _matches;

        public ConverterFamily(Predicate<PropertyInfo> matches, Func<IValueConverterRegistry, PropertyInfo, ValueConverter> builder, string description)
        {
            _matches = matches;
            _builder = builder;
            _description = description;
        }

        public bool Matches(PropertyInfo property)
        {
            return _matches(property);
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return _builder(registry, property);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = _description;
        }
    }
}