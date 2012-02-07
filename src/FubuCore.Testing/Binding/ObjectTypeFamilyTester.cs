using System.Reflection;
using FubuCore.Binding;
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
        private object _value;
        protected override void beforeEach()
        {
            _objectProperty = ReflectionHelper.GetProperty<MyModel>(p => p.ObjectProperty);
            _stringProperty = ReflectionHelper.GetProperty<MyModel>(p => p.StringProperty);
            _value = "value";
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
            ClassUnderTest.Convert(new InMemoryBindingContext().WithPropertyValue(_value)).ShouldEqual(_value);
        }


        public class MyModel
        {
            public object ObjectProperty { get; set; }
            public string StringProperty { get; set; }
        }

    }
}