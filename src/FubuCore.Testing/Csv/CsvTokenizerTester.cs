using System.Collections.Generic;
using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class CsvTokenizerTester : InteractionContext<CsvTokenizer>
    {
        [Test]
        public void simple_split()
        {
            tokensFor("1,2,3").ShouldHaveTheSameElementsAs("1", "2", "3");
        }

        [Test]
        public void escaped_commas()
        {
            tokensFor("aaa,\"bb,b\",ccc")
                .ShouldHaveTheSameElementsAs("aaa", "bb,b", "ccc");
        }

        [Test]
        public void escaped_new_lines()
        {
            tokensFor("start,\"middle\r\n\",end")
                .ShouldHaveTheSameElementsAs("start", "middle\r\n", "end");
        }

        [Test]
        public void escaped_double_quotes()
        {
            //"aaa","b""bb","ccc"
            tokensFor("\"aaa\",\"b\"\"bb\",\"ccc\"")
                .ShouldHaveTheSameElementsAs("aaa", "b\"bb", "ccc");
        }

        [Test]
        public void line_end_escape()
        {
            ClassUnderTest.Read("\"end of line escape complete\"");
            ClassUnderTest.Read("\"new line escape started\"");
            ClassUnderTest.Tokens.ShouldHaveTheSameElementsAs("end of line escape complete", "new line escape started");
        }

        private IEnumerable<string> tokensFor(string line)
        {
            ClassUnderTest.Read(line);
            return ClassUnderTest.Tokens;
        }
    }
}