using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.Values;

namespace FubuCore.Csv
{
    public class CsvData
    {
        public CsvData(IEnumerable<string> values)
        {
            Values = values.ToArray();
        }

        public string[] Values { get; private set; }

        public IValueSource ToValueSource(IEnumerable<ColumnDefinition> columns)
        {
            var index = 0;
            var bounds = Values.Length;
            var dictionary = new Dictionary<string, string>();

            columns.Each(col =>
            {
                string value = null;
                if(index < bounds)
                {
                    value = Values[index];
                }

                dictionary.Add(col.Accessor.Name, value);
                ++index;
            });

            return new FlatValueSource(dictionary);
        }
    }
}