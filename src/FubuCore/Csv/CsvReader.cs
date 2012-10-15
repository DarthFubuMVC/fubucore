using System.IO;
using FubuCore.Binding;

namespace FubuCore.Csv
{
    public class CsvReader : ICsvReader
    {
        private readonly IObjectResolver _resolver;

        public CsvReader(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Read<T>(CsvRequest<T> request)
        {
            using (var stream = new FileStream(request.FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    if (!request.IgnoreColumnHeaders)
                    {
                        var headers = reader.ReadLine();
                        if (headers.IsEmpty()) return;
                    }

                    processData(reader, request);
                }
            }
        }

        private void processData<T>(StreamReader reader, CsvRequest<T> request)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var source = request.Mapping.As<IValueSourceProvider>().Build(line);
                var result = _resolver.BindModel(typeof(T), source);

                request.Callback(result.Value.As<T>());
            }
        }
    }
}