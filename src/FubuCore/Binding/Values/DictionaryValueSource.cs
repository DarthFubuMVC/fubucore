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
            if (!HasChild(key))
            {
                var dict = new Dictionary<string, object>();
                _dictionary.Add(key, dict);
            }

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

        public bool Equals(DictionaryValueSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._dictionary, _dictionary) && Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DictionaryValueSource)) return false;
            return Equals((DictionaryValueSource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dictionary != null ? _dictionary.GetHashCode() : 0)*397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }
    }
}