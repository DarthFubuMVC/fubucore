using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class DictionaryValueSourceLoader
    {
        private readonly DictionaryValueSource _source;
        private readonly Cache<DictionaryPath, DictionaryValueSource> _values;

        public DictionaryValueSourceLoader(string name)
        {
            _source = new DictionaryValueSource(new Dictionary<string, object>(), name);
            _values = new Cache<DictionaryPath, DictionaryValueSource>(path => path.GetChild(_source));
        }

        public void Load(string property, object value)
        {
            var path = new DictionaryPath(property);
            _values[path].Set(path.Key, value);
        }

        public DictionaryValueSource Source
        {
            get { return _source; }
        }
    }
}