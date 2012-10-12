using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class CsvFileReaderIntegratedTester
    {
        private CsvFileReader<TestCsvObject> theReader;
        private IObjectResolver theResolver;
        private IList<TestCsvObject> theResults;

        [SetUp]
        public void SetUp()
        {
            using (var writer = new StreamWriter("test.csv"))
            {
                writer.WriteLine("name,count,csv");
                writer.WriteLine("test1,true,1");
                writer.WriteLine("test2,false,2");
            }

            theReader = new CsvFileReader<TestCsvObject>(new TestCsvMapping());
            theResolver = ObjectResolver.Basic();

            theResults = theReader.Read(theResolver, "test.csv").ToList();
        }

        [Test]
        public void reads_the_objects()
        {
            theResults[0].Name.ShouldEqual("test1");
            theResults[0].Flag.ShouldBeTrue();
            theResults[0].Count.ShouldEqual(1);

            theResults[1].Name.ShouldEqual("test2");
            theResults[1].Flag.ShouldBeFalse();
            theResults[1].Count.ShouldEqual(2);
        }
    }
}