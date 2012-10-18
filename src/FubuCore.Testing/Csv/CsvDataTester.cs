using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class CsvDataTester
    {
        [Test]
        public void simple_split()
        {
            valuesFor("1,2,3").ShouldHaveTheSameElementsAs("1", "2", "3");
        }

        [Test]
        public void escaped_commas()
        {
            valuesFor("aaa,\"bb,b\",ccc")
                .ShouldHaveTheSameElementsAs("aaa", "bb,b", "ccc");
        }

        [Test]
        public void escaped_new_lines()
        {
            valuesFor("start,\"middle\r\n\",end")
                .ShouldHaveTheSameElementsAs("start", "middle\r\n", "end");
        }

        [Test]
        public void escaped_double_quotes()
        {
            //"aaa","b""bb","ccc"
            valuesFor("\"aaa\",\"b\"\"bb\",\"ccc\"")
                .ShouldHaveTheSameElementsAs("aaa", "bbb", "ccc");
        }

        private string[] valuesFor(string input)
        {
            return new CsvData(input).Values;
        }
    }
}