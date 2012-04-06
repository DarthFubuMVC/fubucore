using System;
using System.Collections.Generic;
using System.Data;
using FubuCore.Binding.InMemory;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class RowProcessingRequest<T>
    {
        public RowProcessingRequest()
        {
            Callback = x => { };
        }

        public Func<IDataReader, T> Finder { get; set; }
        public Action<T> Callback { get; set; }
        public IDataReader Reader { get; set; }
    }

    public class ReaderBinder
    {
        private readonly Cache<string, string> _aliases = new Cache<string, string>(key => key);
        private readonly IObjectResolver _binder;
        private readonly IServiceLocator _services;

        public ReaderBinder(IObjectResolver binder, IServiceLocator services)
        {
            _binder = binder;
            _services = services;
        }

        public IEnumerable<T> Build<T>(Func<IDataReader> getReader) where T : new()
        {
            using (IDataReader reader = getReader())
            {
                return Build<T>(reader);
            }
        }

        public IEnumerable<T> Build<T>(IDataReader reader) where T : new()
        {
            var list = new List<T>();

            Build(new RowProcessingRequest<T>
            {
                Callback = list.Add,
                Finder = r => new T(),
                Reader = reader
            });

            return list;
        }

        public void Build<T>(RowProcessingRequest<T> input)
        {
            IDataReader reader = input.Reader;

            // TODO -- awkward!  Let's do some convenience methods here and make this easier
            var request = new DataReaderValues(reader, _aliases);
            var context = new BindingContext(new RequestData(new FlatValueSource(request)), _services, new NulloBindingLogger());

            while (reader.Read())
            {
                T target = input.Finder(reader);
                _binder.BindProperties(target, context);

                input.Callback(target);
            }
        }

        public void SetAlias(string name, string alias)
        {
            _aliases[name] = alias;
        }
    }
}