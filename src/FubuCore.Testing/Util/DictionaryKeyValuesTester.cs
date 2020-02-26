using System;
using System.Collections.Generic;
using FubuCore.Util;
using NUnit.Framework;
using FubuTestingSupport;
using NSubstitute;

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

            var action = Substitute.For<Action<string, string>>();

            values.ForValue("random", action).ShouldBeFalse();

            action.ReceivedWithAnyArgs(0).Invoke(null, null);
        }

        [Test]
        public void ForValue_hit()
        {
            var values = new DictionaryKeyValues();
            values.Dictionary.Add("something", "else");

            var action = Substitute.For<Action<string, string>>();

            values.ForValue("something", action).ShouldBeTrue();

            action.Received().Invoke("something", "else");
        }
    }
}