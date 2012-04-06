using System.IO;
using System.Text;
using FubuCore.Binding.InMemory;
using FubuCore.Binding.Values;
using FubuCore.Util;


namespace FubuCore.Binding
{
    public class FlatFileReader<T>
    {
        private readonly IObjectResolver _resolver;
        private readonly IServiceLocator _services;

        public FlatFileReader(IObjectResolver resolver, IServiceLocator services)
        {
            _resolver = resolver;
            _services = services;
        }

        public void ReadFile(FlatFileRequest<T> request)
        {
            using (var stream = new FileStream(request.Filename, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(stream, request.Encoding);
                var headers = reader.ReadLine();
                if (headers.IsEmpty()) return;

                processData(request, reader, headers);
            }
        }

        private void processData(FlatFileRequest<T> request, StreamReader reader, string headers)
        {
            var data = new FlatFileValues(request.Concatenator, headers);
            _aliases.Each((header, alias) => data.Alias(header, alias));

            var context = new BindingContext(new RequestData(new FlatValueSource(data)), _services, new NulloBindingLogger());

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                readTargetFromLine(request, data, line, context);
            }
        }

        private void readTargetFromLine(FlatFileRequest<T> request, FlatFileValues data, string line, BindingContext context)
        {
            data.ReadLine(line);

            var target = request.Finder(new RequestData(new FlatValueSource(data)));
            _resolver.BindProperties(target, context);

            request.Callback(target);
        }

        private readonly Cache<string, string> _aliases = new Cache<string, string>();
        public void Alias(string header, string alias)
        {
            _aliases[header] = alias;
        }
    }
}