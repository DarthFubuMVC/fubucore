using System;
using FubuCore.Configuration;
using Moq;
using NUnit.Framework;

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
            theValues.Has("AppSettings.Nested.Flag3").ShouldBeTrue();
            theValues.Has("a").ShouldBeTrue();
            theValues.Has("AppSettings.Nested.Files[1].Location").ShouldBeTrue();
            

            theValues.Has("not a real value").ShouldBeFalse();
        }

        [Test]
        public void value_miss()
        {
            var action = new Mock<Action<string, string>>();

            theValues.ForValue("random", action.Object).ShouldBeFalse();

            action.Verify(x => x.Invoke(Arg<string>.Is.Anything, Arg<string>.Is.Anything), Times.Never());
        }

        [Test]
        public void value_hit()
        {
            var action = new Mock<Action<string, string>>();

            theValues.ForValue("a", action.Object).ShouldBeTrue();

            action.Verify(x => x.Invoke("a", "1"));
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