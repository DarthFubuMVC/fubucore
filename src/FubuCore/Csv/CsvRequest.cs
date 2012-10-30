using System;

namespace FubuCore.Csv
{
    public class CsvRequest<T>
    {
        public CsvRequest()
        {
            HeadersExist = true;
            UseHeaderOrdering = true;

        	Delimiter = ',';
        }

        public string FileName { get; set; }
        
		public bool HeadersExist { get; set; }
        public bool UseHeaderOrdering { get; set; }
        
		public ColumnMapping<T> Mapping { get; set; }
        public Action<T> Callback { get; set; }

		public char Delimiter { get; set; }
    }
}