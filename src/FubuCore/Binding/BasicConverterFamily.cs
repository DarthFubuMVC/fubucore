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
            description.Title = "IObjectConverter:" + _propertyType.Name;
            description.ShortDescription =
                "IObjectConverter.FromString(text, typeof({0}))".ToFormat(_propertyType.FullName);
        }

        public object Convert(IPropertyContext context)
        {
            if (context.RawValueFromRequest == null || context.RawValueFromRequest.RawValue == null) return _defaulter.Default();


            return context.RawValueFromRequest.RawValue.GetType().CanBeCastTo(_propertyType)
                       ? context.RawValueFromRequest.RawValue
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

        public bool Equals(BasicValueConverter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._propertyType, _propertyType) && Equals(other._strategy, _strategy);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BasicValueConverter)) return false;
            return Equals((BasicValueConverter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_propertyType != null ? _propertyType.GetHashCode() : 0)*397) ^ (_strategy != null ? _strategy.GetHashCode() : 0);
            }
        }
    }
}