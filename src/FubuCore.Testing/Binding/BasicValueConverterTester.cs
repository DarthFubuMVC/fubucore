using System;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuTestingSupport;
using NSubstitute;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicValueConverterTester
    {
        [Test]
        public void create_default_of_values()
        {
            var context = Substitute.For<IPropertyContext>();
            context.RawValueFromRequest.Returns(null as BindingValue);

            new BasicValueConverter(null, typeof (string)).Convert(context)
                .ShouldBeNull();

            new BasicValueConverter(null, typeof (int)).Convert(context)
                .ShouldEqual(default(int));

            new BasicValueConverter(null, typeof (bool)).Convert(context)
                .ShouldEqual(default(bool));
        }

        [Test]
        public void should_get_raw_value_from_binding_value()
        {
            var context = Substitute.For<IPropertyContext>();
            var testValue = "testValue";
            context.RawValueFromRequest.Returns(new BindingValue{RawValue=testValue});

            new BasicValueConverter(null, typeof (string)).Convert(context).ShouldEqual(testValue);
        }

        [Test]
        public void should_call_conversion_strategy_if_value_is_not_of_same_property_type()
        {
            var context = Substitute.For<IPropertyContext>();
            var convertStrategy = Substitute.For<IConverterStrategy>();
            var testGuid = Guid.NewGuid();
            var testValue = testGuid.ToString();
            convertStrategy.Convert(context).Returns(testGuid);
            context.RawValueFromRequest.Returns(new BindingValue { RawValue = testValue });

            new BasicValueConverter(convertStrategy, typeof(Guid)).Convert(context).ShouldEqual(testGuid);
        }
    }
}