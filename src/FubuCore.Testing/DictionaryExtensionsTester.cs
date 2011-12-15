using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class DictionaryExtensionsTester
    {
        private Dictionary<string, int> dictionary;

        [SetUp]
        public void SetUp()
        {
            dictionary = new Dictionary<string, int>()
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
            };
        }

        [Test]
        public void get_a_dictionary_value_that_exists()
        {
            dictionary.Get("a").ShouldEqual(1);
        }

        [Test]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value()
        {
            dictionary.Get("d").ShouldEqual(default(int));
        }

        [Test]
        public void get_a_dictionary_value_that_exists_2()
        {
            dictionary.Get("a", 5).ShouldEqual(1);
        }


        [Test]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value_2()
        {
            dictionary.Get("d", 5).ShouldEqual(5);
        }

        [Test]
        public void child()
        {
            var dict = new Dictionary<string, object>();
            var child = new Dictionary<string, object>();
            child.Add("a1", 1);

            dict.Add("child1", child);

            dict.Child("child1").ShouldBeTheSameAs(child);


        }

        [Test]
        public void get_simple()
        {
            var dict = new Dictionary<string, object>{
                {"a", 1},
                {"b", true},
                {"c", false}
            };

            dict.Get<int>("a").ShouldEqual(1);
            dict.Get<bool>("b").ShouldBeTrue();
            dict.Get<bool>("c").ShouldBeFalse();
        }

        [Test]
        public void get_complex()
        {
            var leaf = new Dictionary<string, object>{
                {"a", 1},
                {"b", true},
                {"c", false}
            };

            var node = new Dictionary<string, object>{
                {"leaf", leaf}
            };

            var top = new Dictionary<string, object>{
                {"node", node}
            };

            top.Get<int>("node/leaf/a").ShouldEqual(1);
            top.Get<bool>("node/leaf/b").ShouldBeTrue();
            top.Get<bool>("node/leaf/c").ShouldBeFalse();
        }

    }
}