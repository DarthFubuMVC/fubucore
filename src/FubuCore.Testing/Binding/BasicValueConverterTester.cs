using System;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicValueConverterTester
    {
        [Test]
        public void create_default_of_values()
        {
            var context = MockRepository.GenerateMock<IPropertyContext>();
            context.Stub(x => x.RawValueFromRequest).Return(null);

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
            var context = MockRepository.GenerateMock<IPropertyContext>();
            var testValue = "testValue";
            context.Stub(x => x.RawValueFromRequest).Return(new BindingValue{RawValue=testValue});

            new BasicValueConverter(null, typeof (string)).Convert(context).ShouldEqual(testValue);
        }

        [Test]
        public void should_call_conversion_strategy_if_value_is_not_of_same_property_type()
        {
            var context = MockRepository.GenerateMock<IPropertyContext>();
            var convertStrategy = MockRepository.GenerateMock<IConverterStrategy>();
            var testGuid = Guid.NewGuid();
            var testValue = testGuid.ToString();
            convertStrategy.Stub(s => s.Convert(context)).Return(testGuid);
            context.Stub(x => x.RawValueFromRequest).Return(new BindingValue { RawValue = testValue });

            new BasicValueConverter(convertStrategy, typeof(Guid)).Convert(context).ShouldEqual(testGuid);
        }
    }
}