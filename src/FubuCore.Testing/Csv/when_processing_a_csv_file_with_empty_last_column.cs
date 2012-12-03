using System.IO;
using FubuCore.Csv;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class when_processing_a_csv_file_with_empty_last_column : CsvReaderHarness<TestCsvMapping, TestCsvObject>
    {
        protected override void writeFile(StreamWriter writer)
        {
            writer.WriteLine("Count|Flag|Name");
            writer.WriteLine("1|true|");
        }

        protected override void configureRequest(CsvRequest<TestCsvObject> request)
        {
            request.HeadersExist = true;
            request.UseHeaderOrdering = true;

            request.Delimiter = '|';
        }

        [Test]
        public void parses_the_last_value()
        {
            var t1 = new TestCsvObject { Name = null, Count = 1, Flag = true };

            theResultsAre(t1);
        }
    }
}