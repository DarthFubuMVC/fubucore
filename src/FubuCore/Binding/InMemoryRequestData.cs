using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class InMemoryRequestData : RequestDataBase
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public InMemoryRequestData()
        {
        }

        public InMemoryRequestData(IDictionary<string, object> values)
        {
            _values = new Cache<string, object>(values);
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        protected override object fetch(string key)
        {
            return _values[key];
        }

        protected override bool hasValue(string key)
        {
            return _values.Has(key);
        }

        public override IEnumerable<string> GetKeys()
        {
            return _values.GetAllKeys();
        }

        protected override string source
        {
            get { return "in memory"; }
        }

        public void ReadData(string text)
        {
            new StringPropertyReader(text).ReadProperties((key, value) => _values[key] = value);
        }
    }
}