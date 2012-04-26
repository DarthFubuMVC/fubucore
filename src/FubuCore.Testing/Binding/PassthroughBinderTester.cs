using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class PassthroughBinderTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void matches_by_property_type_positive()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            binder.Matches(property(x => x.File)).ShouldBeTrue();
        }

        [Test]
        public void matches_by_property_type_negative()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            binder.Matches(property(x => x.File2)).ShouldBeFalse();
        }

        [Test]
        public void matches_by_object_property_type_negative()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            binder.Matches(property(x => x.File3)).ShouldBeFalse();
        }

        [Test]
        public void build_passes_through()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            var context = MockRepository.GenerateMock<IPropertyContext>();
            context.Expect(c => c.RawValueFromRequest).Return(new BindingValue() { RawValue = new object() });
            ValueConverter converter = binder.Build(MockRepository.GenerateStub<IValueConverterRegistry>(), property(x => x.File));
            converter.Convert(context);
            context.VerifyAllExpectations();
        }

        [Test]
        public void detects_BindingValue_and_returns_inner_value()
        {
            var testValue = "testValue";
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            var context = MockRepository.GenerateMock<IPropertyContext>();
            context.Stub(c => c.RawValueFromRequest).Return(new BindingValue() { RawValue = testValue });
            binder.Convert(context).ShouldBeTheSameAs(testValue);
        }

        private PropertyInfo property(Expression<Func<ModelWithHttpPostedFileBase, object>> expression)
        {
            return ReflectionHelper.GetProperty(expression);
        }
    }

    public class ModelWithHttpPostedFileBase
    {
        public HttpPostedFileBase File { get; set; }
        public string File2 { get; set; }
        public object File3 { get; set; }
    }
}