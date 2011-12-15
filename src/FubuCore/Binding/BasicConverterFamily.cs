using System;
using System.Reflection;
using FubuCore.Conversion;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class BasicConverterFamily : IConverterFamily
    {
        private readonly IObjectConverter _converter;


        public BasicConverterFamily(IObjectConverter converter)
        {
            if (converter == null) throw new ArgumentNullException("converter");

            _converter = converter;
        }

        public bool Matches(PropertyInfo property)
        {
            return _converter.CanBeParsed(property.PropertyType);
        }


        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            var strategy = _converter.StrategyFor(propertyType);
            return new BasicValueConverter(strategy, propertyType);
        }
    }

    public class BasicValueConverter : ValueConverter
    {
        private readonly IDefaultMaker _defaulter;
        private readonly IConverterStrategy _strategy;
        private readonly Type _propertyType;

        public BasicValueConverter(IConverterStrategy strategy, Type propertyType)
        {
            _strategy = strategy;
            _propertyType = propertyType;
            _defaulter = typeof (DefaultMaker<>).CloseAndBuildAs<IDefaultMaker>(propertyType);
        }

        public object Convert(IPropertyContext context)
        {
            if (context.PropertyValue == null) return _defaulter.Default();


            return context.PropertyValue.GetType().CanBeCastTo(_propertyType)
                       ? context.PropertyValue
                       : _strategy.Convert(context);
        }

        public class DefaultMaker<T> : IDefaultMaker
        {
            public object Default()
            {
                return default(T);
            }
        }

        public interface IDefaultMaker
        {
            object Default();
        }
    }
}