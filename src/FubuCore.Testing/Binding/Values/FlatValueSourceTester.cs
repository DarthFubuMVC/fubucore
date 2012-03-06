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
    public class FlatValueSourceTester
    {
        private Dictionary<string, string> theDictionary;
        private FlatValueSource theValues;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, string>();

            theValues = new FlatValueSource(theDictionary, "some name");
        }

        [Test]
        public void the_name_property()
        {
            theValues.Provenance.ShouldEqual("some name");
        }

        [Test]
        public void has()
        {
            theValues.Has("a").ShouldBeFalse();
            theDictionary.Add("a", "1");

            theValues.Has("a").ShouldBeTrue();
            theValues.Has("b").ShouldBeFalse();
        }

        [Test]
        public void get()
        {
            theDictionary.Add("a", "1");
            theDictionary.Add("b", "2");

            theValues.Get("a").ShouldEqual("1");
            theValues.Get("b").ShouldEqual("2");
        }

        [Test]
        public void has_child()
        {
            theValues.HasChild("child1").ShouldBeFalse();
            theDictionary.Add("child1prop1", "1");

            theValues.HasChild("child1").ShouldBeTrue();
            theValues.HasChild("child2").ShouldBeFalse();
        }

        [Test]
        public void get_children_if_there_are_no_matching_values()
        {
            theValues.GetChildren("List").Any().ShouldBeFalse();
        }

        [Test]
        public void get_children_for_each()
        {
            theDictionary.Add("List[0]Prop1", "01");
            theDictionary.Add("List[0]Prop2", "02");
            theDictionary.Add("List[1]Prop1", "11");
            theDictionary.Add("List[2]Prop1", "21");
            theDictionary.Add("List[3]Prop1", "31");

            var children = theValues.GetChildren("List");

            children.Count().ShouldEqual(4);
            children.ElementAt(0).Get("Prop1").ShouldEqual("01");
            children.ElementAt(0).Get("Prop2").ShouldEqual("02");
            children.ElementAt(1).Get("Prop1").ShouldEqual("11");
        }

        [Test]
        public void write_the_report()
        {
            var report = MockRepository.GenerateMock<IValueReport>();

            theDictionary.Add("a", "1");
            theDictionary.Add("b", "2");
            theDictionary.Add("c", "3");
            theDictionary.Add("d", "4");
            theDictionary.Add("e", "5");

            theValues.WriteReport(report);

            report.AssertWasCalled(x => x.Value("a", "1"));
            report.AssertWasCalled(x => x.Value("b", "2"));
            report.AssertWasCalled(x => x.Value("c", "3"));
            report.AssertWasCalled(x => x.Value("d", "4"));
            report.AssertWasCalled(x => x.Value("e", "5"));
        }

        [Test]
        public void value_hit()
        {
            theDictionary.Add("a", "1");

            var action = MockRepository.GenerateMock<Action<BindingValue>>();

            theValues.Value("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(new BindingValue{
                RawKey = "a",
                RawValue = "1",
                Source = theValues.Provenance
            }));
        }
    }

    [TestFixture]
    public class when_fetching_a_child_value_source
    {
        private Dictionary<string, string> theDictionary;
        private FlatValueSource theValues;
        private IValueSource child;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, string>();

            theValues = new FlatValueSource(theDictionary, "some name");

            theDictionary.Add("ChildProp1", "1");
            theDictionary.Add("ChildProp2", "2");
            theDictionary.Add("ChildProp3", "3");
            theDictionary.Add("ChildProp4", "4");
            theDictionary.Add("ChildDescProp1", "123");

            child = theValues.GetChild("Child");
        }

        [Test]
        public void can_get_immediate_properties_of_the_child()
        {
            child.Has("Prop100").ShouldBeFalse();
            child.Has("Prop1").ShouldBeTrue();

            child.Get("Prop1").ShouldEqual("1");
            child.Get("Prop2").ShouldEqual("2");
            child.Get("Prop3").ShouldEqual("3");
            child.Get("Prop4").ShouldEqual("4");
        }

        [Test]
        public void grandchild_mechanics()
        {
            child.HasChild("random").ShouldBeFalse();
            child.HasChild("Desc").ShouldBeTrue();

            var grandchild = child.GetChild("Desc");
            grandchild.Has("Random").ShouldBeFalse();
            grandchild.Has("Prop1").ShouldBeTrue();

            grandchild.Get("Prop1").ShouldEqual("123");
        }


    }

    
}