using System.IO;
using FubuCore.Csv;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
	[TestFixture]
	public class when_processing_a_csv_file_with_non_default_delimiter : CsvReaderHarness<TestCsvMapping, TestCsvObject>
	{
		protected override void writeFile(StreamWriter writer)
		{
			writer.WriteLine("Count|Flag|Name");
			writer.WriteLine("1|true|test1");
			writer.WriteLine("2|false|test2");
		}

		protected override void configureRequest(CsvRequest<TestCsvObject> request)
		{
			request.HeadersExist = true;
			request.UseHeaderOrdering = true;

			request.Delimiter = '|';
		}

		[Test]
		public void respects_the_header_ordering()
		{
			var t1 = new TestCsvObject { Name = "test1", Count = 1, Flag = true };
			var t2 = new TestCsvObject { Name = "test2", Count = 2, Flag = false };

			theResultsAre(t1, t2);
		}
	}
}