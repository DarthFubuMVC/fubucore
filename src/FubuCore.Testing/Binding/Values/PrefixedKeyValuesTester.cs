using FubuCore.Binding.Values;
using NUnit.Framework;
using FubuTestingSupport;

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
    }
}