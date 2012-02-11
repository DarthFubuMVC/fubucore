using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class FlatFileRequestData : RequestDataBase
    {
        private readonly string _concatenator;
        private readonly Cache<string, int> _indices = new Cache<string, int>();
        private string[] _values;

        public FlatFileRequestData(string concatenator, string headerLine)
        {
            _concatenator = concatenator;
            var headers = headerLine.Split(new string[] { _concatenator }, StringSplitOptions.None);
            for (int i = 0; i < headers.Length; i++)
            {
                _indices[headers[i]] = i;
            }
        }

        public void ReadLine(string line)
        {
            _values = line.Split(new string[] { _concatenator }, StringSplitOptions.None);
        }

        protected override object fetch(string key)
        {
            return _values[_indices[key]];
        }

        protected override bool hasValue(string key)
        {
            return _indices.Has(key);
        }

        public void Alias(string header, string alias)
        {
            _indices.WithValue(header, i => _indices[alias] = i);
        }

        public override IEnumerable<string> GetKeys()
        {
            return _indices.GetAllKeys();
        }
    }
}