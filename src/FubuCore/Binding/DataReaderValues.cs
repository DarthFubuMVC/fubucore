using System;
using System.Collections.Generic;
using System.Data;
using FubuCore.Util;
using System.Linq;

namespace FubuCore.Binding
{
    // Tested thru ReaderBinder tests
    public class DataReaderValues : IKeyValues
    {
        private readonly Cache<string, string> _aliases = new Cache<string, string>(key => key);
        private readonly Dictionary<string, string> _columns;
        private readonly IDataReader _reader;

        public DataReaderValues(IDataReader reader, Cache<string, string> aliases) : this(reader)
        {
            aliases.OnMissing = key => key;
            _aliases = aliases;
        }

        public DataReaderValues(IDataReader reader)
        {
            _reader = reader;
            _columns = new Dictionary<string, string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                _columns.Add(reader.GetName(i), null);
            }
        }

        public void SetAlias(string name, string alias)
        {
            _aliases[name] = alias;
        }

        public bool Has(string key)
        {
            return _columns.ContainsKey(key) || _aliases.Has(key);
        }

        public string Get(string key)
        {
            var rawValue = _reader[_aliases[key]];
            return rawValue == DBNull.Value ? null : rawValue.ToString();
        }

        public IEnumerable<string> GetKeys()
        {
            return _columns.Keys.Union(_aliases.GetAllKeys());
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (!Has(key)) return false;

            callback(key, Get(key));

            return true;
        }
    }
}