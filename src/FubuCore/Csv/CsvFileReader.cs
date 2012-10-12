using System.Collections.Generic;
using System.IO;
using FubuCore.Binding;

namespace FubuCore.Csv
{
    public class CsvFileReader<T>
    {
        private ColumnMapping<T> _mapping;

        public CsvFileReader(ColumnMapping<T> mapping)
        {
            _mapping = mapping;
        }

        public IEnumerable<T> Read(IObjectResolver resolver, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var headers = reader.ReadLine();
                    if (headers.IsEmpty()) yield break;

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var source = _mapping.As<IValueSourceProvider>().Build(line);
                        var result = resolver.BindModel(typeof (T), source);

                        yield return result.Value.As<T>();
                    }
                }
            }
        }
    }
}