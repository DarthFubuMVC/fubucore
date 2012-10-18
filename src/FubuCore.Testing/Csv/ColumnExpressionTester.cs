using FubuCore.Csv;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class ColumnExpressionTester
    {
        private Accessor theAccessor;
        private ColumnDefinition theDefinition;
        private ColumnExpression theExpression;

        [SetUp]
        public void SetUp()
        {
            theAccessor = ReflectionHelper.GetAccessor<TestCsvObject>(x => x.Name);
            theDefinition = new ColumnDefinition(theAccessor);
            theExpression = new ColumnExpression(theDefinition);
        }

        [Test]
        public void aliases_the_definition()
        {
            theExpression.Alias("Test");
            theDefinition.Alias.ShouldEqual("Test");
        }

        [Test]
        public void alias_the_definition_from_the_accessor()
        {
            theExpression.Alias(x => x.Name.ToLower());
            theDefinition.Alias.ShouldEqual(theAccessor.Name.ToLower());
        }
    }
}