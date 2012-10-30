using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.Values;

namespace FubuCore.Csv
{
    // See the CsvReaderHarness for integration tests against this
    public class CsvReader : ICsvReader
    {
        private readonly IObjectResolver _resolver;
        private readonly CsvTokenizer _tokenizer;

        public CsvReader(IObjectResolver resolver)
        {
            _resolver = resolver;
            _tokenizer = new CsvTokenizer();
        }

        public void Read<T>(CsvRequest<T> request)
        {
            using (var stream = new FileStream(request.FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
					_tokenizer.DelimitBy(request.Delimiter);

                    var headers = determineHeaders(reader, request);
                    processData(reader, headers, request);
                }
            }
        }

        private CsvData determineHeaders<T>(TextReader reader, CsvRequest<T> request)
        {
            CsvData headers = null;
            if (request.HeadersExist)
            {
                readUntilComplete(reader, tokens =>
                {
                    if (tokens.Any() && request.UseHeaderOrdering)
                        headers = new CsvData(tokens);
                    return false;
                });
            }

            return headers;
        }

        private void processData<T>(TextReader reader, CsvData headers, CsvRequest<T> request)
        {
            var mapping = request.Mapping.As<IColumnMapping>();
            readUntilComplete(reader, tokens =>
            {
                var source = valueSourceFor(tokens, headers, mapping);
                var result = _resolver.BindModel(typeof(T), source);

                request.Callback(result.Value.As<T>());
                return true;
            });
        }

        private void readUntilComplete(TextReader reader, Func<IEnumerable<string>, bool> onComplete)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _tokenizer.Read(line);
                if (_tokenizer.IsPendingForMoreLines && reader.Peek() > 0) continue;

                _tokenizer.MarkReadComplete();
                var canContinue = onComplete(_tokenizer.Tokens);
                _tokenizer.Reset();
                if (!canContinue) break;
            }
        }

        private static IValueSource valueSourceFor(IEnumerable<string> values, CsvData headers, IColumnMapping mapping)
        {
            var data = new CsvData(values);
            return headers == null
                       ? mapping.ValueSource(data)
                       : mapping.ValueSource(data, headers);
        }
    }
}