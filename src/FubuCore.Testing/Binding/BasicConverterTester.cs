using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Testing.Conversion;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class HandlesEmptyBindingNumericFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new BindingRegistry();
            _property = typeof(PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.AllConverterFamilies().FirstOrDefault(cf =>
                cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = new Mock<IPropertyContext>();
            _context.Setup(x => x.Property).Returns(_property);
            _propertyValue = new BindingValue { RawValue = "" };
            _context.Setup(c => c.RawValueFromRequest).Returns(_propertyValue);//.Repeat.Times(4);
        }

        #endregion

        private BindingRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private Mock<IPropertyContext> _context;
        private BindingValue _propertyValue;

        private class PropertyHolder
        {
            public decimal Property { get; set; }
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                BindingScenario<PropertyHolder>.Build(x => x.Data("Property", ""))
                    .Property.ShouldEqual(0);
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
    public class USCultureNumericFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new BindingRegistry();
            _property = typeof (PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.AllConverterFamilies().FirstOrDefault(cf =>
                                                                   cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = new Mock<IPropertyContext>();
            _context.Setup(x => x.Property).Returns(_property);
            _propertyValue = new BindingValue { RawValue = "1,000.001" };
            _context.Setup(c => c.RawValueFromRequest).Returns(_propertyValue);//.Repeat.Times(4);
        }

        #endregion

        private BindingRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private Mock<IPropertyContext> _context;
        private BindingValue _propertyValue;

        private class PropertyHolder
        {
            public decimal Property { get; set; }
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                BindingScenario<PropertyHolder>.Build(x => x.Data("Property", "1,000.001"))
                    .Property.ShouldEqual(1000.001m);
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
            _registry = new BindingRegistry();
            _property = typeof (PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.AllConverterFamilies().FirstOrDefault(cf =>
                                                                   cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = new Mock<IPropertyContext>();
            _context.Setup(x => x.Property).Returns(_property);
            _propertyValue = new BindingValue { RawValue = "1.000,001" };
            _context.Setup(c => c.RawValueFromRequest).Returns(_propertyValue);//.Repeat.Times(4);
        }

        #endregion

        private BindingRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private Mock<IPropertyContext> _context;
        private BindingValue _propertyValue;

        private class PropertyHolder
        {
            public Decimal Property { get; set; }
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("de-DE")))
            {
                BindingScenario<PropertyHolder>.Build(x =>
                {
                    x.Data("Property", "1.000,001");
                }).Property.ShouldEqual(1000.001m);
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