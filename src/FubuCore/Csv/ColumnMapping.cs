using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Binding.Values;
using FubuCore.Reflection;

namespace FubuCore.Csv
{
    public abstract class ColumnMapping<T> : IColumnMapping
    {
        private readonly IList<ColumnDefinition> _columns = new List<ColumnDefinition>();

        public ColumnExpression Column(Expression<Func<T, object>> expression)
        {
            return Column(expression.ToAccessor());
        }

        public ColumnExpression Column(Accessor accessor)
        {
            var column = new ColumnDefinition(accessor);
            _columns.Add(column);

            return new ColumnExpression(column);
        }

        IEnumerable<ColumnDefinition> IColumnMapping.Columns()
        {
            return _columns;
        }

        ColumnDefinition IColumnMapping.ColumnFor(string alias)
        {
            return _columns.SingleOrDefault(x => x.Name == alias);
        }

        ColumnDefinition IColumnMapping.ColumnFor(Accessor accessor)
        {
            return _columns.SingleOrDefault(x => x.Accessor.Equals(accessor));
        }

        IValueSource IColumnMapping.ValueSource(CsvValues data)
        {
            return sourceFor(_columns, data);
        }

        IValueSource IColumnMapping.ValueSource(CsvValues data, CsvValues headers)
        {
            var columns = headers
                .Values
                .Select(x => ((IColumnMapping)this).ColumnFor(x));

            return sourceFor(columns, data);
        }

        private IValueSource sourceFor(IEnumerable<ColumnDefinition> columns, CsvValues data)
        {
            var index = 0;
            var dictionary = new Dictionary<string, string>();
            columns.Each(col =>
            {
                // TODO -- Harden this
                dictionary.Add(col.Accessor.Name, data.Values[index]);
                ++index;
            });

            return new FlatValueSource(dictionary);
        }
    }
}