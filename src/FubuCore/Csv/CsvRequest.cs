using System;
using System.IO;

namespace FubuCore.Csv
{
    public class CsvRequest<T>
    {
        public CsvRequest()
        {
            HeadersExist = true;
            UseHeaderOrdering = true;

        	Delimiter = ',';

            OpenStream = () => new FileStream(FileName, FileMode.Open, FileAccess.Read);
        }

        public string FileName { get; set; }
        
		public bool HeadersExist { get; set; }
        public bool UseHeaderOrdering { get; set; }
        
		public ColumnMapping<T> Mapping { get; set; }
        public Action<T> Callback { get; set; }

		public char Delimiter { get; set; }

        public Func<Stream> OpenStream { get; set; } 
    }
}