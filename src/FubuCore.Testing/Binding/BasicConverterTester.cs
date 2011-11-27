using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Reflection;
using FubuCore.Testing.Conversion;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicConverterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theObjectConverter = new ObjectConverter();
            _registry = new ValueConverterRegistry(new IConverterFamily[0], theObjectConverter);
            _property = typeof (PropertyHolder).GetProperty("Property");
            _basicConverterFamily = _registry.Families.SingleOrDefault(cf =>
                                                                       cf.Matches(_property)) as BasicConverterFamily;
            _basicConverterFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "some value";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(3);
        }

        #endregion

        private ValueConverterRegistry _registry;
        private BasicConverterFamily _basicConverterFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;
        private ObjectConverter theObjectConverter;

        private class PropertyHolder
        {
            public string Property { get; set; }
            public PropertyHolder Parent { get; set; }
        }

        [Test]
        public void should_build()
        {
            var converter = _basicConverterFamily.Build(_registry, _property);
            converter.Convert(_context).ShouldEqual(_propertyValue);
            _context.VerifyAllExpectations();
        }

        [Test]
        public void should_match_property()
        {
            _basicConverterFamily.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_on_exception()
        {
            var parent = ReflectionHelper.GetProperty<PropertyHolder>(x => x.Parent);
            _basicConverterFamily.Matches(parent).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class USCultureNumericFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0], new ObjectConverter());
            _property = typeof (PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.Families.FirstOrDefault(cf =>
                                                                   cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "1,000.001";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(4);
        }

        #endregion

        private ValueConverterRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;

        private class PropertyHolder
        {
            public decimal Property { get; set; }
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                var converter = _numericTypeFamily.Build(_registry, _property);
                converter.Convert(_context).ShouldEqual(1000.001m);
                _context.VerifyAllExpectations();
            }
        }

        [Test]
        public void should_match_property()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
                _numericTypeFamily.Matches(_property).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class GermanCultureNumericFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0], new ObjectConverter());
            _property = typeof (PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.Families.FirstOrDefault(cf =>
                                                                   cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "1.000,001";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(4);
        }

        #endregion

        private ValueConverterRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;

        private class PropertyHolder
        {
            public Decimal Property { get; set; }
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("de-DE")))
            {
                var converter = _numericTypeFamily.Build(_registry, _property);
                converter.Convert(_context).ShouldEqual(1000.001m);
                _context.VerifyAllExpectations();
            }
        }

        [Test]
        public void should_match_property()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("de-DE")))
                _numericTypeFamily.Matches(_property).ShouldBeTrue();
        }
    }
}