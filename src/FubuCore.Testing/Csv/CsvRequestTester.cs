using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class CsvRequestTester
    {
        private CsvRequest<TestCsvObject> theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = new CsvRequest<TestCsvObject>();
        }

        [Test]
        public void defaults_to_headers_existing()
        {
            theRequest.HeadersExist.ShouldBeTrue();
        }

        [Test]
        public void defaults_to_using_header_ordering()
        {
            theRequest.UseHeaderOrdering.ShouldBeTrue();
        }

		[Test]
		public void defaults_to_comma_delimiter()
		{
			theRequest.Delimiter.ShouldEqual(',');
		}
    }
}