using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Csv
{
    public class CsvData
    {
        public CsvData(IEnumerable<string> values)
        {
            Values = values.ToArray();
        }

        public string[] Values { get; private set; } 
    }
}