using System;
using System.Collections.Generic;
using FubuCore.Util;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

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

            var action = MockRepository.GenerateMock<Action<string, string>>();

            values.ForValue("random", action).ShouldBeFalse();

            action.AssertWasNotCalled(x => x.Invoke(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void ForValue_hit()
        {
            var values = new DictionaryKeyValues();
            values.Dictionary.Add("something", "else");

            var action = MockRepository.GenerateMock<Action<string, string>>();

            values.ForValue("something", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("something", "else"));
        }
    }
}