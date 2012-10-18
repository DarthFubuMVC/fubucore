using System.IO;
using FubuCore.Csv;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class when_processing_a_csv_file_with_headers_and_aliases : CsvReaderHarness<TestAliasMapping, TestCsvObject>
    {
        protected override void writeFile(StreamWriter writer)
        {
            writer.WriteLine("the_flag,Count,the_name");
            writer.WriteLine("true,1,test1");
            writer.WriteLine("false,2,test2");
        }

        protected override void configureRequest(CsvRequest<TestCsvObject> request)
        {
            request.HeadersExist = true;
            request.UseHeaderOrdering = true;
        }

        [Test]
        public void respects_the_aliased_ordering()
        {
            var t1 = new TestCsvObject { Name = "test1", Count = 1, Flag = true };
            var t2 = new TestCsvObject { Name = "test2", Count = 2, Flag = false };

            theResultsAre(t1, t2);
        }
    }

    public class TestAliasMapping : ColumnMapping<TestCsvObject>
    {
        public TestAliasMapping()
        {
            Column(x => x.Flag).Alias("the_flag");
            
            Column(x => x.Name).Alias(x => "the_" + x.Name.ToLower());

            Column(x => x.Count);
        }
    }
}