using System.IO;
using FubuCore.Csv;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class when_processing_a_csv_file_with_headers_that_are_ignored : CsvReaderHarness<TestCsvMapping, TestCsvObject>
    {
        protected override void writeFile(StreamWriter writer)
        {
            writer.WriteLine("name,count,csv");
            writer.WriteLine("test1,true,1");
            writer.WriteLine("test2,false,2");
        }

        protected override void configureRequest(CsvRequest<TestCsvObject> request)
        {
            request.HeadersExist = true;
            request.UseHeaderOrdering = false;
        }

        [Test]
        public void reads_objects_without_the_headers()
        {
            var t1 = new TestCsvObject {Name = "test1", Count = 1, Flag = true};
            var t2 = new TestCsvObject { Name = "test2", Count = 2, Flag = false };

            theResultsAre(t1, t2);
        }
    }
}