using System;
using System.Collections.Generic;

namespace FubuCore.Util
{
    public class DictionaryKeyValues : IKeyValues
    {
        private readonly IDictionary<string, string> _dictionary;

        public DictionaryKeyValues() : this(new Dictionary<string, string>())
        {
        }

        public DictionaryKeyValues(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        public IDictionary<string, string> Dictionary
        {
            get { return _dictionary; }
        }

        public bool Has(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public string Get(string key)
        {
            return _dictionary[key];
        }

        public IEnumerable<string> GetKeys()
        {
            return _dictionary.Keys;
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (!Has(key)) return false;

            callback(key, Get(key));

            return true;
        }
    }
}