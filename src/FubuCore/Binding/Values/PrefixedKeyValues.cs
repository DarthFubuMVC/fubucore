using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{


    public class PrefixedKeyValues<T> : IKeyValues<T>
    {
        private readonly string _prefix;
        private readonly IKeyValues<T> _inner;

        public PrefixedKeyValues(string prefix, IKeyValues<T> inner)
        {
            _prefix = prefix;
            _inner = inner;
        }

        public bool Has(string key)
        {
            return _inner.Has(_prefix + key);
        }

        public T Get(string key)
        {
            return _inner.Get(_prefix + key);
        }

        public IEnumerable<string> GetKeys()
        {
            var matchingKeys = _inner.GetKeys().Where(x => x.StartsWith(_prefix));
            return matchingKeys.Select(x => x.Substring(_prefix.Length));
        }

        public bool ForValue(string key, Action<string, T> callback)
        {
            return _inner.ForValue(_prefix + key, callback);
        }
    }

    public class PrefixedKeyValues : PrefixedKeyValues<string>
    {
        public PrefixedKeyValues(string prefix, IKeyValues<string> inner) : base(prefix, inner)
        {
        }
    }
}