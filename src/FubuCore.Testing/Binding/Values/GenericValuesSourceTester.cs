using System;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class GenericValuesSourceTester
    {
        private GenericValueSource theSource;

        [SetUp]
        public void SetUp()
        {
            theSource = new GenericValueSource("some name", key => key + "1", () => new[]{"a", "b", "c"});
        }

        [Test]
        public void provenance_is_just_the_name()
        {
            theSource.Provenance.ShouldEqual("some name");
        }

        [Test]
        public void has()
        {
            theSource.Has("a").ShouldBeTrue();
            theSource.Has("b").ShouldBeTrue();
            theSource.Has("c").ShouldBeTrue();
            theSource.Has("d").ShouldBeFalse();
        }

        [Test]
        public void get()
        {
            theSource.Get("a").ShouldEqual("a1");
            theSource.Get("b").ShouldEqual("b1");
        }

        [Test]
        public void get_children_returns_an_empty_enumerable()
        {
            theSource.GetChildren("anything").Any().ShouldBeFalse();
        }

        [Test]
        public void value_miss()
        {
            theSource.Value("something missing", v => Assert.Fail("Shouldn't be here")).ShouldBeFalse();
        }

        [Test]
        public void value_hit()
        {
            var action = MockRepository.GenerateMock<Action<BindingValue>>();

            theSource.Value("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(new BindingValue{
                RawKey = "a",
                RawValue = "a1",
                Source = theSource.Provenance
            }));
        }

        [Test]
        public void write_report()
        {
            var report = MockRepository.GenerateMock<IValueReport>();

            theSource.WriteReport(report);

            report.AssertWasCalled(x => x.Value("a", "a1"));
            report.AssertWasCalled(x => x.Value("b", "b1"));
            report.AssertWasCalled(x => x.Value("c", "c1"));
        }
    }
}