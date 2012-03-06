using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Configuration;

namespace FubuCore.Binding.Values
{
    public class DictionaryPath
    {
        private readonly string _parent;
        private readonly string _key;
        private readonly IEnumerable<string> _parentParts;

        public DictionaryPath(string path)
        {
            var parts = path.Trim().Split('.');
            _parentParts = parts.Reverse().Skip(1).Reverse();
            _parent = parts.Any()
                          ? _parentParts.Join(".")
                          : String.Empty;

            _key = parts.Last();
        }

        public void Set(SettingsData top, object value)
        {
            GetParentSource(top).Set(Key, value);
        }

        public SettingsData GetParentSource(SettingsData source)
        {
            ParentParts.Each(x =>
            {
                if (x.Contains("["))
                {
                    var parts = x.TrimEnd(']').Split('[');
                    var index = int.Parse(parts.Last());

                    source = source.GetChildrenElement(parts.First(), index);
                }
                else
                {
                    source = source.Child(x);
                }
                
            });

            return source;
        }

        public IEnumerable<string> ParentParts
        {
            get { return _parentParts; }
        }

        public string Parent
        {
            get { return _parent; }
        }

        public bool Equals(DictionaryPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._parent, _parent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DictionaryPath)) return false;
            return Equals((DictionaryPath) obj);
        }

        public override int GetHashCode()
        {
            return (_parent != null ? _parent.GetHashCode() : 0);
        }

        public string Key
        {
            get { return _key; }
        }
    }
}