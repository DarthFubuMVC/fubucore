using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class FlatValueSource<T> : IValueSource
    {
        private readonly string _provenance;
        private readonly IKeyValues<T> _values;


        public FlatValueSource(IDictionary<string, T> dictionary, string provenance = "Anonymous") : this(new DictionaryKeyValues<T>(dictionary), provenance)
        {
            
        }

        public FlatValueSource(IKeyValues<T> values, string provenance = "Anonymous")
        {
            _provenance = provenance;
            _values = values;
        }

        public string Provenance
        {
            get { return _provenance; }
        }

        public bool Has(string key)
        {
            return _values.Has(key);
        }

        public object Get(string key)
        {
            return _values.Get(key);
        }

        public bool HasChild(string key)
        {
            var enumerable = _values.GetKeys();
            return enumerable.Any(x => x.StartsWith(key));
        }

        public IValueSource GetChild(string key)
        {
            return new FlatValueSource<T>(new PrefixedKeyValues<T>(key, _values));
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            var indexer = new Indexer(key);

            while (HasChild(indexer.Prefix))
            {
                yield return GetChild(indexer.Prefix);
                indexer.Increment();
            }
        }

        public void WriteReport(IValueReport report)
        {
            _values.GetKeys().ToList().Each(x => report.Value(x, _values.Get(x)));
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _values.ForValue(key, (rawKey, value) =>
            {
                callback(new BindingValue
                {
                    RawKey = rawKey,
                    RawValue = value,
                    Source = Provenance
                });
            });
        }


        public class Indexer
        {
            private readonly string _name;
            private int _index = 0;
            private string _prefix;

            public Indexer(string name)
            {
                _name = name;
                setPrefix();
            }

            private void setPrefix()
            {
                _prefix = "{0}[{1}]".ToFormat(_name, _index);
            }

            public void Increment()
            {
                _index++;
                setPrefix();
            }

            public string Prefix
            {
                get { return _prefix; }
            }
        }
    }



    public class FlatValueSource : FlatValueSource<string>
    {
        public FlatValueSource(IDictionary<string, string> dictionary, string name = "Anonymous") : this(new DictionaryKeyValues(dictionary), name){}

        public FlatValueSource(IKeyValues values, string name = "Anonymous")
            : base(values, name)
        {
        }
    }
}