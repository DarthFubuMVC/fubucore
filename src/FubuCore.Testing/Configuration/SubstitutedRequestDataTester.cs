using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SubstitutedRequestDataTester
    {
        private Dictionary<string, string> theDictionary;
        private InMemoryRequestData theInnerData;
        private SubstitutedRequestData theSubstitutedData;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, string>();
            theInnerData = new InMemoryRequestData();

            theSubstitutedData = new SubstitutedRequestData(theInnerData, theDictionary);
        }

        [Test]
        public void return_null_if_the_inner_is_null()
        {
            theSubstitutedData.Value("something that does not exist").ShouldBeNull();
        }

        [Test]
        public void return_a_value_that_is_not_templated()
        {
            theInnerData["Key"] = "1";
            theSubstitutedData.Value("Key").ShouldEqual("1");
        }

        [Test]
        public void return_a_templated_value()
        {
            theDictionary.Add("setting", "setting-value");
            theInnerData["Key"] = "*{setting}*";

            theSubstitutedData.Value("Key").ShouldEqual("*setting-value*");
        }

        [Test]
        public void do_nothing_for_value_that_does_not_exist_CPS_style()
        {
            theSubstitutedData.Value("Key", o => Assert.Fail("shouldn't be here")).ShouldBeFalse();
        }

        [Test]
        public void return_template_method_from_CPS_style()
        {
            theDictionary.Add("setting", "setting-value");
            theInnerData["Key"] = "*{setting}*";

            var action = MockRepository.GenerateMock<Action<object>>();

            theSubstitutedData.Value("Key", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("*setting-value*"));
        }
    }
}