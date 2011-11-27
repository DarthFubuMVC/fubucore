using System;
using System.Reflection;
using FubuCore.Conversion;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class BasicConverterFamily : IConverterFamily
    {
        private readonly IObjectConverter _converter;

        private readonly Cache<Type, ValueConverter> _valueConverters
            = new Cache<Type, ValueConverter>();

        public BasicConverterFamily(IObjectConverter converter)
        {
            if (converter == null) throw new ArgumentNullException("converter");

            _converter = converter;

            _valueConverters.OnMissing = type => new BasicValueConverter(_converter, type);
        }

        public bool Matches(PropertyInfo property)
        {
            return _converter.CanBeParsed(property.PropertyType);
        }


        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            return _valueConverters[propertyType];
        }
    }

    public class BasicValueConverter : ValueConverter
    {
        private readonly IObjectConverter _converter;
        private readonly IDefaultMaker _defaulter;
        private readonly Type _propertyType;

        public BasicValueConverter(IObjectConverter converter, Type propertyType)
        {
            _converter = converter;
            _propertyType = propertyType;
            _defaulter = typeof (DefaultMaker<>).CloseAndBuildAs<IDefaultMaker>(propertyType);
        }

        public object Convert(IPropertyContext context)
        {
            var propertyType = context.Property.PropertyType;

            if (context.PropertyValue == null) return _defaulter.Default();


            return context.PropertyValue.GetType() == propertyType
                       ? context.PropertyValue
                       : _converter.FromString(context.PropertyValue.ToString(), _propertyType);
        }

        #region Nested type: DefaultMaker

        public class DefaultMaker<T> : IDefaultMaker
        {
            public object Default()
            {
                return default(T);
            }
        }

        #endregion

        #region Nested type: IDefaultMaker

        public interface IDefaultMaker
        {
            object Default();
        }

        #endregion
    }
}