using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BooleanFamilyTester
    {
        public class DummyClass
        {
            public bool Hungry { get; set; }
            public bool? Thirsty { get; set; }
        }

        [Test]
        public void can_accept_the_property_name_and_treat_it_as_true()
        {
            BindingScenario<DummyClass>.For(x =>
            {
                x.Data(o => o.Hungry, "Hungry");
            }).Model.Hungry.ShouldBeTrue();
        }

        [Test]
        public void can_convert_boolean_values()
        {
            new BooleanFamily().Matches(ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry)).ShouldBeTrue();
        }

        [Test]
        public void can_convert_nullable_boolean_values()
        {
            new BooleanFamily().Matches(ReflectionHelper.GetProperty<DummyClass>(c => c.Thirsty)).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class BooleanFamily_can_convert_various_string_inputs_to_boolean
    {
        private bool WithValue(string value)
        {
            return BindingScenario<BooleanTarget>.For(x =>
            {
                x.Data(o => o.IsTrue, value);

            }).Model.IsTrue;
        }

        [Test]
        public void treats_false_string_as_false()
        {
            WithValue("false").ShouldBeFalse();
            WithValue("False").ShouldBeFalse();
            WithValue("FALSE").ShouldBeFalse();
        }

        [Test]
        public void treats_true_string_as_true()
        {
            WithValue("true").ShouldBeTrue();
            WithValue("True").ShouldBeTrue();
            WithValue("TRUE").ShouldBeTrue();
        }

        [Test]
        public void treats_on_string_as_true()
        {
            WithValue("on").ShouldBeTrue();
            WithValue("ON").ShouldBeTrue();
        }

        [Test]
        public void treats_no_string_as_false()
        {
            WithValue("no").ShouldBeFalse();
            WithValue("NO").ShouldBeFalse();
            WithValue("n").ShouldBeFalse();
            WithValue("N").ShouldBeFalse();
        }

        [Test]
        public void treats_yes_string_as_true()
        {
            WithValue("yes").ShouldBeTrue();
            WithValue("YES").ShouldBeTrue();
            WithValue("y").ShouldBeTrue();
            WithValue("Y").ShouldBeTrue();
        }

        [Test]
        public void treats_empty_string_as_false()
        {
            WithValue("").ShouldBeFalse();
        }

        public class BooleanTarget
        {
            public bool IsTrue { get; set; }
        }
    }
}