using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class GenericKeyValues : IKeyValues
    {
        private readonly Func<string, string> _source;
        private readonly Func<IEnumerable<string>> _allKeys;

        public GenericKeyValues(Func<string, string> source, Func<IEnumerable<string>> allKeys)
        {
            _source = source;
            _allKeys = allKeys;
        }

        public bool Has(string key)
        {
            return _allKeys().Contains(key);
        }

        public string Get(string key)
        {
            return _source(key);
        }

        public IEnumerable<string> GetKeys()
        {
            return _allKeys();
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (Has(key))
            {
                callback(key, Get(key));
                return true;
            }

            return false;
        }
    }
}