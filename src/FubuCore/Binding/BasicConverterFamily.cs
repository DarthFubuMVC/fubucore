using System;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using System.Collections.Generic;

namespace FubuCore.Binding
{
    [Description("Delegates to IObjectConverter for the conversion")]
    public class BasicConverterFamily : IConverterFamily, DescribesItself
    {
        private readonly ConverterLibrary _library;


        public BasicConverterFamily(ConverterLibrary library)
        {
            if (library == null) throw new ArgumentNullException("library");

            _library = library;
        }

        public bool Matches(PropertyInfo property)
        {
            return _library.CanBeParsed(property.PropertyType);
        }


        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            var strategy = _library.StrategyFor(propertyType);
            return new BasicValueConverter(strategy, propertyType);
        }

        public void Describe(Description description)
        {
            var libraryDesc = Description.For(_library);
            description.BulletLists.AddRange(libraryDesc.BulletLists);
        }
    }

    public class BasicValueConverter : ValueConverter, DescribesItself
    {
        private readonly IDefaultMaker _defaulter;
        private readonly Type _propertyType;
        private readonly IConverterStrategy _strategy;

        public BasicValueConverter(IConverterStrategy strategy, Type propertyType)
        {
            _strategy = strategy;
            _propertyType = propertyType;
            _defaulter = typeof (DefaultMaker<>).CloseAndBuildAs<IDefaultMaker>(propertyType);
        }

        public void Describe(Description description)
        {
            description.Title = "Using IObjectConverter";
            description.ShortDescription =
                "IObjectConverter.FromString(text, typeof({0}))".ToFormat(_propertyType.FullName);
        }

        public object Convert(IPropertyContext context)
        {
            if (context.PropertyValue == null) return _defaulter.Default();


            return context.PropertyValue.GetType().CanBeCastTo(_propertyType)
                       ? context.PropertyValue
                       : _strategy.Convert(context);
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