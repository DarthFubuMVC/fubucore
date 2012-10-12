using System;
using System.Linq.Expressions;
using FubuCore.Binding.Values;
using FubuCore.Csv;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    public class TestCsvMapping : ColumnMapping<TestCsvObject>
    {
        public TestCsvMapping()
        {
            Column(x => x.Name);
            Column(x => x.Flag);
            Column(x => x.Count);
        }
    }

    [TestFixture]
    public class ColumnMappingTester
    {
        private TestCsvMapping theMapping;
        private string theRawValues;
        private IValueSource theValues;

        [SetUp]
        public void SetUp()
        {
            theMapping = new TestCsvMapping();
            theRawValues = "Test,true,1";

            theValues = theMapping.As<IValueSourceProvider>().Build(theRawValues);
        }

        [Test]
        public void the_values_are_indexed_by_accessor()
        {
            get(x => x.Name).ShouldEqual("Test");
            get(x => x.Flag).ShouldEqual("true");
            get(x => x.Count).ShouldEqual("1");
        }

        private object get(Expression<Func<TestCsvObject, object>> expression)
        {
            return theValues.Get(expression.ToAccessor().Name);
        }
    }
}