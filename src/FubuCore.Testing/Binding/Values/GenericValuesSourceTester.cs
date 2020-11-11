using System;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using Moq;
using NUnit.Framework;

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
            var action = new Mock<Action<BindingValue>>();

            theSource.Value("a", action.Object).ShouldBeTrue();

            action.Verify(x => x.Invoke(new BindingValue{
                RawKey = "a",
                RawValue = "a1",
                Source = theSource.Provenance
            }));
        }

        [Test]
        public void write_report()
        {
            var report = new Mock<IValueReport>();

            theSource.WriteReport(report.Object);

            report.Verify(x => x.Value("a", "a1"));
            report.Verify(x => x.Value("b", "b1"));
            report.Verify(x => x.Value("c", "c1"));
        }
    }
}