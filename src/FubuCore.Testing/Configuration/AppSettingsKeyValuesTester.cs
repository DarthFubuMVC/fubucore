using System;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class AppSettingsKeyValuesTester
    {
        private AppSettingsKeyValues theValues;

        [SetUp]
        public void SetUp()
        {
            theValues = new AppSettingsKeyValues();
        }

        [Test]
        public void contains_key()
        {
            theValues.ContainsKey("AppSettings.Nested.Flag3").ShouldBeTrue();
            theValues.ContainsKey("a").ShouldBeTrue();
            theValues.ContainsKey("AppSettings.Nested.Files[1].Location").ShouldBeTrue();
            

            theValues.ContainsKey("not a real value").ShouldBeFalse();
        }

        [Test]
        public void value_miss()
        {
            var action = MockRepository.GenerateMock<Action<string, string>>();

            theValues.ForValue("random", action).ShouldBeFalse();

            action.AssertWasNotCalled(x => x.Invoke(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void value_hit()
        {
            var action = MockRepository.GenerateMock<Action<string, string>>();

            theValues.ForValue("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("a", "1"));
        }

        [Test]
        public void get()
        {
            theValues.Get("a").ShouldEqual("1");
            theValues.Get("AppSettings.Flag1").ShouldEqual("f1");
        }

        [Test]
        public void get_all_keys()
        {
            var keys = theValues.GetKeys();
            keys.ShouldContain("a");
            keys.ShouldContain("b");
            keys.ShouldContain("c");
            keys.ShouldContain("AppSettings.Flag1");
        }
    }
}