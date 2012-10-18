using FubuCore.Csv;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class ColumnDefinitionTester
    {
        private Accessor theAccessor;
        private ColumnDefinition theDefinition;

        [SetUp]
        public void SetUp()
        {
            theAccessor = ReflectionHelper.GetAccessor<TestCsvObject>(x => x.Name);
            theDefinition = new ColumnDefinition(theAccessor);
        }

        [Test]
        public void uses_the_accessor_name_if_no_alias_is_specified()
        {
            theDefinition.Name.ShouldEqual(theAccessor.Name);
        }

        [Test]
        public void uses_the_alias_when_specified()
        {
            theDefinition.Alias = "Test";
            theDefinition.Name.ShouldEqual(theDefinition.Alias);
        }
    }
}