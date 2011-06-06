using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class OverrideSubstitutedRequestDataTester
    {
        private Dictionary<string, string> theDictionary;
        private InMemoryRequestData theInnerData;
        private SubstitutedRequestData theSubstitutedData;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, string>();
            theInnerData = new InMemoryRequestData();

            //inner / substitutions
            theSubstitutedData = new SubstitutedRequestData(theInnerData, new DictionaryKeyValues(theDictionary));

            theDictionary.Add("val", "value");
            theDictionary.Add("setting", "setting-{val}");
            theInnerData["Key"] = "*{setting}*";
        }

        

        [Test]
        public void return_a_templated_value()
        {
            theSubstitutedData.Value("Key").ShouldEqual("*setting-value*");
        }
    }
}