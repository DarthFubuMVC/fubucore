using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class AppSettingsRequestDataIntegratedTester
    {
        private AppSettingsRequestData theData;

        [SetUp]
        public void SetUp()
        {
            theData = new AppSettingsRequestData(typeof(AppSettings));
        }

        [Test]
        public void value()
        {
            theData.Value("Flag1").ShouldEqual("f1");
            theData.Value("Flag2").ShouldEqual("f2");
        }

        [Test]
        public void value_of_complicated_values()
        {
            theData.Value("NestedFlag3").ShouldEqual("f3");
        }

        [Test]
        public void get_subrequest()
        {
            theData.GetSubRequest("Nested").Value("Flag3").ShouldEqual("f3");
        }

        [Test]
        public void get_enumerable_requests()
        {
            theData.GetEnumerableRequests("NestedFiles").ShouldHaveCount(2);
        }
    }
}