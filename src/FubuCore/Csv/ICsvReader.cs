using System;
using FubuCore.Binding;

namespace FubuCore.Csv
{
    public interface ICsvReader
    {
        void Read<T>(string fileName, ColumnMapping<T> mapping, Action<T> continuation);
    }

    public class CsvReader : ICsvReader
    {
        private readonly IObjectResolver _resolver;

        public CsvReader(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Read<T>(string fileName, ColumnMapping<T> mapping, Action<T> continuation)
        {
            var reader = new CsvFileReader<T>(mapping);
            foreach (var item in reader.Read(_resolver, fileName))
            {
                continuation(item);
            }
        }
    }
}