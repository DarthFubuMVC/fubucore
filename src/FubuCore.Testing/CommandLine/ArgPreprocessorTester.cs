using FubuCore.CommandLine;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class ArgPreprocessorTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void should_split_multi_args()
        {
            ArgPreprocessor.Process(new[] {"-abc"}).ShouldHaveTheSameElementsAs("-a", "-b", "-c");
        }

        [Test]
        public void should_ignore_long_flag_args()
        {
            ArgPreprocessor.Process(new[] {"--abc"}).ShouldHaveTheSameElementsAs("--abc");
        }

        [Test]
        public void should_support_multiple_types_of_flags()
        {
            ArgPreprocessor.Process(new[] { "-abc", "--xyz", "b" }).ShouldHaveTheSameElementsAs("-a", "-b", "-c", "--xyz", "b");
        }

    }
}