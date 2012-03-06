using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class DictionaryPathTester
    {
        private Dictionary<string, object> theDictionary;
        private SettingsData theSource;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, object>();
            theSource = new SettingsData(theDictionary);
        }

        [Test]
        public void parse_a_one_level_property()
        {
            var path = new DictionaryPath("A.a");
            path.Set(theSource, 1);

            theDictionary.Child("A")["a"].ShouldEqual(1);
        }

        [Test]
        public void parse_a_two_level_property()
        {
            var path = new DictionaryPath("A.B.a");
            path.Set(theSource, 1);

            theDictionary.Child("A").Child("B")["a"].ShouldEqual(1);
        }

        [Test]
        public void parse_a_three_level_property()
        {
            var path = new DictionaryPath("A.B.C.a");
            path.Set(theSource, 1);

            theDictionary.Child("A").Child("B").Child("C")["a"].ShouldEqual(1);
        }

        [Test]
        public void parse_with_enumerable()
        {
            new DictionaryPath("A[0].Name").Set(theSource, "Jeremy");
            new DictionaryPath("A[3].Name").Set(theSource, "Shiner"); // Our dog's name.  Yes, we're Texans
            new DictionaryPath("A[2].Name").Set(theSource, "Max");
            new DictionaryPath("A[1].Name").Set(theSource, "Lindsey");

            theDictionary.Children("A").ElementAt(0)["Name"].ShouldEqual("Jeremy");
            theDictionary.Children("A").ElementAt(1)["Name"].ShouldEqual("Lindsey");
            theDictionary.Children("A").ElementAt(2)["Name"].ShouldEqual("Max");
            theDictionary.Children("A").ElementAt(3)["Name"].ShouldEqual("Shiner");
        
        }

        [Test]
        public void parse_with_enumerable_deep_graph()
        {
            new DictionaryPath("A[2].Nested.B[3].Name").Set(theSource, "Monte");

            theDictionary.Children("A").ElementAt(2).Child("Nested").Children("B").ElementAt(3)
                ["Name"].ShouldEqual("Monte");
        }
    }
}