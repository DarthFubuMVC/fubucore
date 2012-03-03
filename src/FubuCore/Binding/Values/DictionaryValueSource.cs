using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding.Values
{
    public class DictionaryValueSource : IValueSource
    {
        private readonly IDictionary<string, object> _dictionary;
        private readonly string _name;

        public DictionaryValueSource(IDictionary<string, object> dictionary, string name = "Anonymous")
        {
            _dictionary = dictionary;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Has(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public object Get(string key)
        {
            return _dictionary[key];
        }

        public bool HasChild(string key)
        {
            return Has(key)
                       ? _dictionary[key] is IDictionary<string, object>
                       : false;
        }

        public IValueSource GetChild(string key)
        {
            var childDict = _dictionary.Child(key);
            return new DictionaryValueSource(childDict, Name + "." + key);
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            if (!Has(key)) return Enumerable.Empty<IValueSource>();

            var enumerable = _dictionary[key] as IEnumerable<IDictionary<string, object>>;
            if (enumerable == null)
            {
                return Enumerable.Empty<IValueSource>();
            }

            var i = 0;
            return enumerable.Select(x =>
            {
                var name = "{0}.{1}[{2}]".ToFormat(_name, key, i);

                i++;
                return new DictionaryValueSource(x, name);
            }).ToList();
        }

        public void WriteReport(IValueReport report)
        {
            throw new NotImplementedException();
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            if (!Has(key)) return false;

            callback(new BindingValue(){
                RawKey = key,
                RawValue = _dictionary[key],
                Source = _name
            });

            return true;
        }
    }
}