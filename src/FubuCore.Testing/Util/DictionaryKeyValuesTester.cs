using System;
using FubuCore.Util;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class DictionaryKeyValuesTester
    {
        [Test]
        public void ForValue_miss()
        {
            var values = new DictionaryKeyValues();
            values.Dictionary.Add("something", "else");

            var action = new Mock<Action<string, string>>();

            values.ForValue("random", action.Object).ShouldBeFalse();

            action.VerifyNotCalled(x => x.Invoke(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [Test]
        public void ForValue_hit()
        {
            var values = new DictionaryKeyValues();
            values.Dictionary.Add("something", "else");

            var action = new Mock<Action<string, string>>();

            values.ForValue("something", action.Object).ShouldBeTrue();

            action.Verify(x => x.Invoke("something", "else"));
        }
    }
}