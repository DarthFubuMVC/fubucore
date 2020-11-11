using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding.Values;
using FubuCore.Csv;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public class creating_a_value_source_when_not_all_values_exist
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

            theColumns.Add(new ColumnDefinition(accessor(x => x.Name)));
            theColumns.Add(new ColumnDefinition(accessor(x => x.Flag)));
            theColumns.Add(new ColumnDefinition(accessor(x => x.Count)));
        }

        private IValueSource theValues { get { return new CsvData(theData).ToValueSource(theColumns); } }

        [Test]
        public void the_value_is_null_if_it_doesnt_exist()
        {
            get(x => x.Count).ShouldBeNull();
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