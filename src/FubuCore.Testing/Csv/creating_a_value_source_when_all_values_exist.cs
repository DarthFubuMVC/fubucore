using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding.Values;
using FubuCore.Csv;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class creating_a_value_source_when_all_values_exist
    {
        private IList<ColumnDefinition> theColumns;
        private IList<string> theData;
            
        [SetUp]
        public void SetUp()
        {
            theData = new List<string>();
            theColumns = new List<ColumnDefinition>();

            theData.Add("Test");
            theData.Add("true");
            theData.Add("1");

            theColumns.Add(new ColumnDefinition(accessor(x => x.Name)));
            theColumns.Add(new ColumnDefinition(accessor(x => x.Flag)));
            theColumns.Add(new ColumnDefinition(accessor(x => x.Count)));
        }

        private IValueSource theValues { get { return new CsvData(theData).ToValueSource(theColumns); } }

        [Test]
        public void the_values_are_indexed_by_accessor()
        {
            get(x => x.Name).ShouldEqual("Test");
            get(x => x.Flag).ShouldEqual("true");
            get(x => x.Count).ShouldEqual("1");
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
}