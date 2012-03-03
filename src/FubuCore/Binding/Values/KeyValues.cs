using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class KeyValues : IKeyValues
    {
        private readonly Cache<string, string> _values = new Cache<string, string>();

        public string this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }


        public bool Has(string key)
        {
            return _values.Has(key);
        }

        public string Get(string key)
        {
            return _values[key];
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.GetAllKeys();
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (!Has(key)) return false;

            callback(key, Get(key));

            return true;
        }

        public void ReadData(string text)
        {
            StringPropertyReader.ForText(text).ReadProperties((key, value) => _values[key] = value);
        }
    }
}