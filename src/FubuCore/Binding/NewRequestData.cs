using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.Values;

namespace FubuCore.Binding
{
    public class NewRequestData : IRequestData
    {
        private readonly IList<IValueSource> _sources = new List<IValueSource>();

        public NewRequestData(IValueSource source)
        {
            _sources.Add(source);
        }

        public NewRequestData(IEnumerable<IValueSource> sources)
        {
            _sources.AddRange(sources);
        }

        public object Value(string key)
        {
            object output = null;

            Value(key, val => output = val.RawValue);

            return output;
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _sources.Any(x => x.Value(key, callback));
        }

        public bool HasChildRequest(string key)
        {
            return _sources.Any(x => x.HasChild(key));
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            var sources = _sources.Where(x => x.HasChild(prefixOrChild)).Select(x => x.GetChild(prefixOrChild));
            return new NewRequestData(sources);
        }

        // Living with the limitation that only *ONE* source can ever have 
        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            foreach (var valueSource in _sources)
            {
                var childrenSources = valueSource.GetChildren(prefixOrChild);
                if (childrenSources.Any())
                {
                    return childrenSources.Select(x => new NewRequestData(x)).ToList();
                }
            }

            return Enumerable.Empty<IRequestData>();
        }
    }

}