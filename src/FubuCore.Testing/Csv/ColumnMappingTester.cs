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

        public TestCsvMapping(Action<TestCsvMapping> configure)
        {
            configure(this);
        }
    }

    [TestFixture]
    public class ColumnMappingTester
    {
        private IColumnMapping theMapping;
        private string[] theRawValues;
        private IValueSource theValues;

        [SetUp]
        public void SetUp()
        {
            theMapping = new TestCsvMapping();
            theRawValues = new[] { "Test", "true", "1" };

            theValues = theMapping.ValueSource(new CsvData(theRawValues));
        }

        [Test]
        public void the_values_are_indexed_by_accessor()
        {
            get(x => x.Name).ShouldEqual("Test");
            get(x => x.Flag).ShouldEqual("true");
            get(x => x.Count).ShouldEqual("1");
        }

        [Test]
        public void has_columns_for_each_explicit_call()
        {
            theMapping.Columns().ShouldHaveCount(3);

            theMapping.ColumnFor(accessor(x => x.Name)).ShouldNotBeNull();
            theMapping.ColumnFor(accessor(x => x.Flag)).ShouldNotBeNull();
            theMapping.ColumnFor(accessor(x => x.Count)).ShouldNotBeNull();
        }

        [Test]
        public void column_for_alias()
        {
            theMapping.ColumnFor(accessor(x => x.Name).Name).ShouldNotBeNull();
        }

        private Accessor accessor(Expression<Func<TestCsvObject, object>> expression)
        {
            return expression.ToAccessor();
        }

        private object get(Expression<Func<TestCsvObject, object>> expression)
        {
            return theValues.Get(accessor(expression).Name);
        }
    }

    [TestFixture]
    public class building_value_source_by_headers
    {
        private IColumnMapping theMapping;
        private CsvData theRawValues;
        private CsvData theHeaders;
        private IValueSource theValues;

        [SetUp]
        public void SetUp()
        {
            theMapping = new TestCsvMapping();
            theHeaders = new CsvData(new[] { "Count", "Flag", "Name" });
            theRawValues = new CsvData(new[] { "1", "true", "Test" });

            theValues = theMapping.ValueSource(theRawValues, theHeaders);
        }

        [Test]
        public void values_are_indexed_by_header_order()
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

    [TestFixture]
    public class building_value_source_by_headers_with_aliases
    {
        private IColumnMapping theMapping;
        private CsvData theRawValues;
        private CsvData theHeaders;
        private IValueSource theValues;

        [SetUp]
        public void SetUp()
        {
            theMapping = new MappingWithAliases();
            theHeaders = new CsvData(new[] { "Count", "Flag", "SomethingElse" });
            theRawValues = new CsvData(new[] { "1", "true", "Test" });

            theValues = theMapping.ValueSource(theRawValues, theHeaders);
        }

        [Test]
        public void values_are_indexed_by_header_order_with_alias()
        {
            get(x => x.Name).ShouldEqual("Test");
            get(x => x.Flag).ShouldEqual("true");
            get(x => x.Count).ShouldEqual("1");
        }

        private object get(Expression<Func<TestCsvObject, object>> expression)
        {
            return theValues.Get(expression.ToAccessor().Name);
        }

        public class MappingWithAliases : ColumnMapping<TestCsvObject>
        {
            public MappingWithAliases()
            {
                Column(x => x.Name).Alias("SomethingElse");
                Column(x => x.Count);
                Column(x => x.Flag);
            }
        }
    }
}