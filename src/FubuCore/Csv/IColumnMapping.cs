using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Reflection;

namespace FubuCore.Csv
{
    public interface IColumnMapping
    {
        IEnumerable<ColumnDefinition> Columns();

        ColumnDefinition ColumnFor(string alias);
        ColumnDefinition ColumnFor(Accessor accessor);
        
        IValueSource ValueSource(CsvValues data);
        IValueSource ValueSource(CsvValues data, CsvValues headers);
    }
}