using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ObjectTypeFamilyTester : InteractionContext<ObjectTypeFamily>
    {
        private PropertyInfo _objectProperty;
        private PropertyInfo _stringProperty;

        protected override void beforeEach()
        {
            _objectProperty = ReflectionHelper.GetProperty<MyModel>(p => p.ObjectProperty);
            _stringProperty = ReflectionHelper.GetProperty<MyModel>(p => p.StringProperty);
        }

        [Test]
        public void matches_for_property_of_object_type_positive()
        {
            ClassUnderTest.Matches(_objectProperty).ShouldBeTrue();
        }

        [Test]
        public void matches_for_property_of_object_type_negative()
        {
            ClassUnderTest.Matches(_stringProperty).ShouldBeFalse();
        }

        [Test]
        public void convert_returns_the_context_property_value()
        {
            BindingScenario<MyModel>.Build(x =>
            {
                x.Data(o => o.ObjectProperty, 123);
            }).ObjectProperty.ShouldEqual(123);
        }


        public class MyModel
        {
            public object ObjectProperty { get; set; }
            public string StringProperty { get; set; }
        }

    }
}