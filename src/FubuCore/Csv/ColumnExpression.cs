using System;
using FubuCore.Reflection;

namespace FubuCore.Csv
{
    public class ColumnExpression
    {
        private readonly ColumnDefinition _column;

        public ColumnExpression(ColumnDefinition column)
        {
            _column = column;
        }

        public void Alias(string alias)
        {
            _column.Alias = alias;
        }

        public void Alias(Func<Accessor, string> convention)
        {
            Alias(convention(_column.Accessor));
        }
    }
}