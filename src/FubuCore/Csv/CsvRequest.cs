using System;

namespace FubuCore.Csv
{
    public class CsvRequest<T>
    {
        public string FileName { get; set; }
        public bool IgnoreColumnHeaders { get; set; }
        public ColumnMapping<T> Mapping { get; set; }
        public Action<T> Callback { get; set; }
    }
}