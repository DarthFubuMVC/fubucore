using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class PrefixedKeyValues : IKeyValues
    {
        private readonly string _prefix;
        private readonly IKeyValues _inner;

        public PrefixedKeyValues(string prefix, IKeyValues inner)
        {
            _prefix = prefix;
            _inner = inner;
        }

        public bool ContainsKey(string key)
        {
            return _inner.ContainsKey(_prefix + key);
        }

        public string Get(string key)
        {
            return _inner.Get(_prefix + key);
        }

        public IEnumerable<string> GetKeys()
        {
            var prefixChars = _prefix.ToCharArray();
            return _inner.GetKeys().Where(x => x.StartsWith(_prefix)).Select(x => x.TrimStart(prefixChars));
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            return _inner.ForValue(_prefix + key, callback);
        }
    }
}