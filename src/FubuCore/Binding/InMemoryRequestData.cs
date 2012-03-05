using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class InMemoryRequestData : RequestData
    {
        private readonly Cache<string, object> _values;


        private InMemoryRequestData(IDictionary<string, object> values)
            : base(new FlatValueSource<object>(values, "in memory"))
        {
            _values = new Cache<string, object>(values);
        }


        public InMemoryRequestData() : this(new Dictionary<string, object>())
        {
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public void ReadData(string text)
        {
            StringPropertyReader.ForText(text).ReadProperties((key, value) => _values[key] = value);
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.GetAllKeys();
        }
    }
}