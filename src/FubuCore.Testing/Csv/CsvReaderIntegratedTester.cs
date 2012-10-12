using System.Collections.Generic;
using System.IO;
using FubuCore.Binding;
using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class CsvReaderIntegratedTester
    {
        private CsvReader theReader;
        private IList<TestCsvObject> theObjects;

        [SetUp]
        public void SetUp()
        {
            using (var writer = new StreamWriter("test.csv"))
            {
                writer.WriteLine("name,count,csv");
                writer.WriteLine("test1,true,1");
                writer.WriteLine("test2,false,2");
            }

            theReader = new CsvReader(ObjectResolver.Basic());

            theObjects = new List<TestCsvObject>();

            var theMap = new TestCsvMapping();
            theReader.Read("test.csv", theMap, o => theObjects.Add(o));
        }

        [Test]
        public void reads_the_objects()
        {
            theObjects[0].Name.ShouldEqual("test1");
            theObjects[0].Flag.ShouldBeTrue();
            theObjects[0].Count.ShouldEqual(1);

            theObjects[1].Name.ShouldEqual("test2");
            theObjects[1].Flag.ShouldBeFalse();
            theObjects[1].Count.ShouldEqual(2);
        }
    }
}