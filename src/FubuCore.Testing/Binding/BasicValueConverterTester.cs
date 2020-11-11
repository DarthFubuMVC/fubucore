using System;
using FubuCore.Binding;
using FubuCore.Conversion;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicValueConverterTester
    {
        [Test]
        public void create_default_of_values()
        {
            var context = new Mock<IPropertyContext>();
            context.Setup(x => x.RawValueFromRequest).Returns((BindingValue)null);

            new BasicValueConverter(null, typeof (string)).Convert(context.Object)
                .ShouldBeNull();

            new BasicValueConverter(null, typeof (int)).Convert(context.Object)
                .ShouldEqual(default(int));

            new BasicValueConverter(null, typeof (bool)).Convert(context.Object)
                .ShouldEqual(default(bool));
        }

        [Test]
        public void should_get_raw_value_from_binding_value()
        {
            var context = new Mock<IPropertyContext>();
            var testValue = "testValue";
            context.Setup(x => x.RawValueFromRequest).Returns(new BindingValue{RawValue=testValue});

            new BasicValueConverter(null, typeof (string)).Convert(context.Object).ShouldEqual(testValue);
        }

        [Test]
        public void should_call_conversion_strategy_if_value_is_not_of_same_property_type()
        {
            var context = new Mock<IPropertyContext>();
            var convertStrategy = new Mock<IConverterStrategy>();
            var testGuid = Guid.NewGuid();
            var testValue = testGuid.ToString();
            convertStrategy.Setup(s => s.Convert(context.Object)).Returns(testGuid);
            context.Setup(x => x.RawValueFromRequest).Returns(new BindingValue { RawValue = testValue });

            new BasicValueConverter(convertStrategy.Object, typeof(Guid)).Convert(context.Object).ShouldEqual(testGuid);
        }
    }
}