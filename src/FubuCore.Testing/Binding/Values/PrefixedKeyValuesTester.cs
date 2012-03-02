using System;
using FubuCore.Binding.Values;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class PrefixedKeyValuesTester
    {
        private KeyValues theValues;
        private PrefixedKeyValues thePrefixedValues;

        [SetUp]
        public void SetUp()
        {
            theValues = new KeyValues();
            thePrefixedValues = new PrefixedKeyValues("One", theValues);
        }

        [Test]
        public void contains_key_has_to_respect_the_prefix()
        {
            theValues["Key1"] = "a";

            thePrefixedValues.ContainsKey("Key1").ShouldBeFalse();

            theValues["OneKey1"] = "b";

            thePrefixedValues.ContainsKey("Key1").ShouldBeTrue();
        }

        [Test]
        public void get_has_to_respect_the_prefix()
        {
            theValues["Key1"] = "a";
            theValues["OneKey1"] = "b";

            thePrefixedValues.Get("Key1").ShouldEqual("b");
        }

        [Test]
        public void get_all_keys_must_respect_the_prefix()
        {
            theValues["Key1"] = "a";
            theValues["Key2"] = "a";
            theValues["Key3"] = "a";
            theValues["OneKey4"] = "a";
            theValues["OneKey5"] = "a";
            theValues["OneKey6"] = "a";

            thePrefixedValues.GetKeys().ShouldHaveTheSameElementsAs("Key4", "Key5", "Key6");
        }

        [Test]
        public void value_miss()
        {
            var action = MockRepository.GenerateMock<Action<string, string>>();

            theValues["Key1"] = "a";
            theValues["Key2"] = "a";
            theValues["Key3"] = "a";

            thePrefixedValues.ForValue("Key1", action).ShouldBeFalse();

            action.AssertWasNotCalled(x => x.Invoke(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void value_hit()
        {
            theValues["Key1"] = "a";
            theValues["Key2"] = "a";
            theValues["Key3"] = "a";
            theValues["OneKey4"] = "a4";
            theValues["OneKey5"] = "a";
            theValues["OneKey6"] = "a";

            var action = MockRepository.GenerateMock<Action<string, string>>();

            thePrefixedValues.ForValue("Key4", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("OneKey4", "a4"));
        }

        [Test]
        public void value_hit_grandchild()
        {
            theValues["Key1"] = "a";
            theValues["Key2"] = "a";
            theValues["Key3"] = "a";
            theValues["OneKey4"] = "a4";
            theValues["OneTwoKey11"] = "1211";
            theValues["OneKey5"] = "a";
            theValues["OneKey6"] = "a";

            var action = MockRepository.GenerateMock<Action<string, string>>();
            var grandchild = new PrefixedKeyValues("Two", thePrefixedValues);

            grandchild.ForValue("Key11", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("OneTwoKey11", "1211"));
        }
    }
}