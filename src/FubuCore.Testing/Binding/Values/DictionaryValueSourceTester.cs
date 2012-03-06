using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class ValueSourceLoaderTester
    {
        [Test]
        public void load_several_levels_of_nested_properties()
        {
            var source = new SettingsData(new Dictionary<string, object>(), "some name");

            source.WriteProperty("A", 1);
            source.WriteProperty("B", 2);
            source.WriteProperty("Child.A", 2);
            source.WriteProperty("Child.B", 3);
            source.WriteProperty("Child.Nested.A", 4);
            source.WriteProperty("Child.Nested.B", 5);

            source.Get("A").ShouldEqual(1);
            source.Get("B").ShouldEqual(2);
            source.As<IValueSource>().GetChild("Child").Get("A").ShouldEqual(2);
            source.As<IValueSource>().GetChild("Child").Get("B").ShouldEqual(3);
            source.As<IValueSource>().GetChild("Child").GetChild("Nested").Get("A").ShouldEqual(4);
            source.As<IValueSource>().GetChild("Child").GetChild("Nested").Get("B").ShouldEqual(5);
        }
    }


    [TestFixture]
    public class DictionaryValueSourceTester
    {
        private Dictionary<string, object> theDictionary;
        private SettingsData theSource;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, object>();
            theSource = new SettingsData(theDictionary, "a name");
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

            var childValueSource = theSource.As<IValueSource>().GetChild("child");
            childValueSource.Get("abc").ShouldEqual(123);
        }

        [Test]
        public void get_child_when_it_does_not_exist()
        {
            var childValueSource = theSource.As<IValueSource>().GetChild("child");

            childValueSource.ShouldNotBeNull().ShouldBeOfType<SettingsData>()
                .Name.ShouldEqual("a name.child");

            // It's idempotent on the add
            theSource.As<IValueSource>().GetChild("child").ShouldEqual(childValueSource);
        }

        [Test]
        public void get_child_name()
        {
            var child = new Dictionary<string, object>();
            theDictionary.Add("child", child);

            var childValueSource = theSource.As<IValueSource>().GetChild("child");

            childValueSource.Name.ShouldEqual(theSource.Name + ".child");
        }

        [Test]
        public void get_children_when_there_are_none()
        {
            theSource.As<IValueSource>().GetChildren("nonexistent").Any().ShouldBeFalse();
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

            var children = theSource.As<IValueSource>().GetChildren("list");
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

            var children = theSource.As<IValueSource>().GetChildren("list");
            children.Count().ShouldEqual(3);

            children.ElementAt(0).Name.ShouldEqual("a name.list[0]");
            children.ElementAt(1).Name.ShouldEqual("a name.list[1]");
            children.ElementAt(2).Name.ShouldEqual("a name.list[2]");

        }

        [Test]
        public void value_miss()
        {
            theSource.As<IValueSource>().Value("nonexistent", x => Assert.Fail("Should not call this")).ShouldBeFalse();
        }

        [Test]
        public void value_hit_at_top_level()
        {
            var action = MockRepository.GenerateMock<Action<BindingValue>>();
            theDictionary.Add("a", 1);

            theSource.As<IValueSource>().Value("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(new BindingValue(){
                RawKey = "a",
                RawValue = 1,
                Source = theSource.Name
            }));
        }

        [Test]
        public void can_retrieve_a_child_element()
        {
            var child2 = theSource.GetChildrenElement("children", 2);
            var child1 = theSource.GetChildrenElement("children", 1);
            var child0 = theSource.GetChildrenElement("children", 0);

            child0.Set("a", 0);
            child1.Set("a", 1);
            child2.Set("a", 2);

            theSource.GetChildrenElement("children", 2).Set("b", 22);

            theDictionary.Children("children").ElementAt(0)["a"].ShouldEqual(0);
            theDictionary.Children("children").ElementAt(1)["a"].ShouldEqual(1);
            theDictionary.Children("children").ElementAt(2)["a"].ShouldEqual(2);
            theDictionary.Children("children").ElementAt(2)["b"].ShouldEqual(22);
        }
    }
}