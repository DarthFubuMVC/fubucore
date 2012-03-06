using System;
using System.Collections.Specialized;
using FubuCore.Binding.Values;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class NamedKeyValuesTester
    {
        private NameValueCollection theCollection;
        private NamedKeyValues theValues;

        [SetUp]
        public void SetUp()
        {
            theCollection = new NameValueCollection();
            theValues = new NamedKeyValues(theCollection);
        }

        [Test]
        public void has()
        {
            theValues.Has("a").ShouldBeFalse();
            theCollection.Add("a", "a1");

            theValues.Has("a").ShouldBeTrue();
            theValues.Has("b").ShouldBeFalse();
        }

        [Test]
        public void get()
        {
            theCollection.Add("a", "1");
            theCollection.Add("b", "2");

            theValues.Get("a").ShouldEqual("1");
            theValues.Get("b").ShouldEqual("2");
        }

        [Test]
        public void get_keys()
        {
            theCollection.Add("a", "1");
            theCollection.Add("b", "2");
            theCollection.Add("c", "3");

            theValues.GetKeys().ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void for_value_miss()
        {
            var action = MockRepository.GenerateMock<Action<string, string>>();

            theValues.ForValue("a", action).ShouldBeFalse();
            action.AssertWasNotCalled(x => x.Invoke(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void for_value_hit()
        {
            theCollection.Add("a", "1");

            var action = MockRepository.GenerateMock<Action<string, string>>();

            theValues.ForValue("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("a", "1"));
        }
    }
}