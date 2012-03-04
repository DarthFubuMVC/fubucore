using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using IDictionary = System.Collections.Generic.IDictionary<string, object>;
using Dictionary = System.Collections.Generic.Dictionary<string, object>;

namespace FubuCore.Binding.Values
{
    public class DictionaryValueSource : IValueSource
    {
        private readonly Cache<DictionaryPath, DictionaryValueSource> _children;
        private readonly IDictionary _dictionary;
        private readonly string _name;

        public DictionaryValueSource(IDictionary dictionary, string name = "Anonymous")
        {
            _dictionary = dictionary;
            _name = name;

            _children = new Cache<DictionaryPath, DictionaryValueSource>(path => path.GetParentSource(this));
        }

        public static DictionaryValueSource For(IKeyValues values, string name = "Anonymous")
        {
            var source = new DictionaryValueSource(new Dictionary(), name);
            values.ReadAll(source.WriteProperty);

            return source;
        }

        /// <summary>
        /// Write property to the value source
        /// </summary>
        /// <param name="property">Dotted path:  AppSettings.Child.Nested.Property1</param>
        /// <param name="value"></param>
        public void WriteProperty(string property, object value)
        {
            var path = new DictionaryPath(property);
            _children[path].Set(path.Key, value);
        }

        public string Name
        {
            get { return _name; }
        }

        public void Set(string key, object value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        public bool Has(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public object Get(string key)
        {
            return _dictionary[key];
        }

        public bool HasChild(string key)
        {
            return Has(key)
                       ? _dictionary[key] is IDictionary
                       : false;
        }



        public IValueSource GetChild(string key)
        {
            if (!HasChild(key))
            {
                var dict = new Dictionary();
                _dictionary.Add(key, dict);
            }

            var childDict = _dictionary.Child(key);
            return new DictionaryValueSource(childDict, Name + "." + key);
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            if (!Has(key)) return Enumerable.Empty<IValueSource>();

            var enumerable = _dictionary[key] as IEnumerable<IDictionary>;
            if (enumerable == null)
            {
                return Enumerable.Empty<IValueSource>();
            }

            var i = 0;
            return enumerable.Select(x =>
            {
                var name = "{0}.{1}[{2}]".ToFormat(_name, key, i);

                i++;
                return new DictionaryValueSource(x, name);
            }).ToList();
        }

        public void WriteReport(IValueReport report)
        {
            _dictionary.Keys.ToList().Each(key =>
            {
                var value = _dictionary[key];

                if (value is IDictionary)
                {
                    report.StartChild(key);
                    var child = GetChild(key);
                    child.WriteReport(report);
                    report.EndChild();
                }
                else if (value is IEnumerable<IDictionary>)
                {
                    var children = GetChildren(key).ToList();
                    for (int i = 0; i < children.Count; i++)
                    {
                        report.StartChild(key, i);
                        children[i].WriteReport(report);
                        report.EndChild();
                    }
                }
                else
                {
                    report.Value(key, value);
                }

            });
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            if (!Has(key)) return false;

            callback(new BindingValue(){
                RawKey = key,
                RawValue = _dictionary[key],
                Source = _name
            });

            return true;
        }

        public bool Equals(DictionaryValueSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._dictionary, _dictionary) && Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DictionaryValueSource)) return false;
            return Equals((DictionaryValueSource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dictionary != null ? _dictionary.GetHashCode() : 0)*397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }

        public DictionaryValueSource GetChildrenElement(string key, int index)
        {
            IList<IDictionary> list;

            if (!Has(key) || !(_dictionary[key] is IList<IDictionary>))
            {
                list = new List<IDictionary>();
                _dictionary.Add(key, list);
            }
            else
            {
                list = _dictionary[key] as IList<IDictionary>;
            }

            while (index > list.Count - 1)
            {
                list.Add(new Dictionary());
            }

            return new DictionaryValueSource(list[index]);
        }
    }
}