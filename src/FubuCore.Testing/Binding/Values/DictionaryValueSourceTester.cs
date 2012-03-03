using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class DictionaryValueSourceTester
    {
        private Dictionary<string, object> theDictionary;
        private DictionaryValueSource theSource;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, object>();
            theSource = new DictionaryValueSource(theDictionary, "a name");
        }

        [Test]
        public void has_the_name()
        {
            theSource.Name.ShouldEqual("a name");
        }

        [Test]
        public void has()
        {
            theSource.Has("a").ShouldBeFalse();
            theSource.Has("b").ShouldBeFalse();
        
            theDictionary.Add("a", 1);
            theSource.Has("a").ShouldBeTrue();
            theSource.Has("b").ShouldBeFalse();
        }

        [Test]
        public void get()
        {
            theDictionary.Add("a", 1);
            theDictionary.Add("b", 2);

            theSource.Get("a").ShouldEqual(1);
            theSource.Get("b").ShouldEqual(2);
        }

        [Test]
        public void has_child()
        {
            var child = new Dictionary<string, object>();
            theDictionary.Add("a", "something");
            theDictionary.Add("child", child);

            theSource.HasChild("a").ShouldBeFalse();
            theSource.HasChild("random").ShouldBeFalse();
            theSource.HasChild("child").ShouldBeTrue();
        }

        [Test]
        public void get_child_when_it_already_exists()
        {
            var child = new Dictionary<string, object>();
            theDictionary.Add("a", "something");
            theDictionary.Add("child", child);

            child.Add("abc", 123);

            var childValueSource = theSource.GetChild("child");
            childValueSource.Get("abc").ShouldEqual(123);
        }

        [Test]
        public void get_child_when_it_does_not_exist()
        {
            var childValueSource = theSource.GetChild("child");

            childValueSource.ShouldNotBeNull().ShouldBeOfType<DictionaryValueSource>()
                .Name.ShouldEqual("a name.child");

            // It's idempotent on the add
            theSource.GetChild("child").ShouldEqual(childValueSource);
        }

        [Test]
        public void get_child_name()
        {
            var child = new Dictionary<string, object>();
            theDictionary.Add("child", child);

            var childValueSource = theSource.GetChild("child");

            childValueSource.Name.ShouldEqual(theSource.Name + ".child");
        }

        [Test]
        public void get_children_when_there_are_none()
        {
            theSource.GetChildren("nonexistent").Any().ShouldBeFalse();
        }

        [Test]
        public void get_children_when_there_are_children()
        {
            var list = new List<IDictionary<string, object>>(){
                new Dictionary<string, object>(),
                new Dictionary<string, object>(),
                new Dictionary<string, object>()
            };

            list[0].Add("a", 0);
            list[1].Add("a", 1);
            list[2].Add("a", 2);

            theDictionary.Add("list", list);

            var children = theSource.GetChildren("list");
            children.Count().ShouldEqual(3);

            children.ElementAt(0).Get("a").ShouldEqual(0);
            children.ElementAt(1).Get("a").ShouldEqual(1);
            children.ElementAt(2).Get("a").ShouldEqual(2);

        
        }

        [Test]
        public void get_children_when_there_are_children_name()
        {
            var list = new List<IDictionary<string, object>>(){
                new Dictionary<string, object>(),
                new Dictionary<string, object>(),
                new Dictionary<string, object>()
            };

            list[0].Add("a", 0);
            list[1].Add("a", 1);
            list[2].Add("a", 2);

            theDictionary.Add("list", list);

            var children = theSource.GetChildren("list");
            children.Count().ShouldEqual(3);

            children.ElementAt(0).Name.ShouldEqual("a name.list[0]");
            children.ElementAt(1).Name.ShouldEqual("a name.list[1]");
            children.ElementAt(2).Name.ShouldEqual("a name.list[2]");

        }

        [Test]
        public void value_miss()
        {
            theSource.Value("nonexistent", x => Assert.Fail("Should not call this")).ShouldBeFalse();
        }

        [Test]
        public void value_hit_at_top_level()
        {
            var action = MockRepository.GenerateMock<Action<BindingValue>>();
            theDictionary.Add("a", 1);

            theSource.Value("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(new BindingValue(){
                RawKey = "a",
                RawValue = 1,
                Source = theSource.Name
            }));
        }
    }
}