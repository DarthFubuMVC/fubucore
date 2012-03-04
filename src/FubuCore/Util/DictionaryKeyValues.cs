using System;
using System.Collections.Generic;

namespace FubuCore.Util
{
    public class DictionaryKeyValues<T> : IKeyValues<T>
    {
        private readonly IDictionary<string, T> _dictionary;

        public DictionaryKeyValues()
            : this(new Dictionary<string, T>())
        {
        }

        public DictionaryKeyValues(IDictionary<string, T> dictionary)
        {
            _dictionary = dictionary;
        }

        public IDictionary<string, T> Dictionary
        {
            get { return _dictionary; }
        }

        public bool Has(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public T Get(string key)
        {
            return _dictionary[key];
        }

        public IEnumerable<string> GetKeys()
        {
            return _dictionary.Keys;
        }

        public bool ForValue(string key, Action<string, T> callback)
        {
            if (!Has(key)) return false;

            callback(key, Get(key));

            return true;
        }
    }

    public class DictionaryKeyValues : DictionaryKeyValues<string>, IKeyValues
    {
        public DictionaryKeyValues()
        {
        }

        public DictionaryKeyValues(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}