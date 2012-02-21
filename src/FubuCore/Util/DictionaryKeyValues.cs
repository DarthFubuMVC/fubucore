using System.Collections.Generic;

namespace FubuCore.Util
{
    public class DictionaryKeyValues : IKeyValues
    {
        private readonly IDictionary<string, string> _dictionary;

        public DictionaryKeyValues(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        public bool ContainsKey(string key)
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
    }
}