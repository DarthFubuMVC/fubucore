using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
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
            return _columns.SingleOrDefault(x => x.Name.EqualsIgnoreCase(alias));
        }

        ColumnDefinition IColumnMapping.ColumnFor(Accessor accessor)
        {
            return _columns.SingleOrDefault(x => x.Accessor.Equals(accessor));
        }

        IValueSource IColumnMapping.ValueSource(CsvData data)
        {
            return data.ToValueSource(_columns);
        }

        IValueSource IColumnMapping.ValueSource(CsvData data, CsvData headers)
        {
            var mapping = this.As<IColumnMapping>();
            var badColumns = new List<string>();

            var columns = headers
                .Values
                .Select(x => {
                    var column = mapping.ColumnFor(x);
                    if (column == null)
                    {
                        badColumns.Add(x);
                    }

                    return column;
                }).ToArray();

            if (badColumns.Any())
            {
                throw new CsvColumnException(badColumns);
            }

            return data.ToValueSource(columns);
        }
    }

    [Serializable]
    public class CsvColumnException : Exception
    {
        public CsvColumnException(IEnumerable<string> columns) : base("Unrecognized columns:  " + columns.Join(", "))
        {
        }

        protected CsvColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}