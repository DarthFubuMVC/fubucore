using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SettingsData : IValueSource
    {
        private readonly Cache<DictionaryPath, SettingsData> _children;
        private readonly IDictionary<string, object> _dictionary;

        public SettingsData() : this(SettingCategory.core)
        {
        }

        public SettingsData(SettingCategory category) : this(new Dictionary<string, object>())
        {
            Category = category;
        }

        public SettingsData(IDictionary<string, object> dictionary, string name = "Anonymous")
        {
            Category = SettingCategory.core;
            _dictionary = dictionary;
            Provenance = name;

            _children = new Cache<DictionaryPath, SettingsData>(path => path.GetParentSource(this));
        }

        public static SettingsData For(IKeyValues values, string name = "Anonymous")
        {
            var source = new SettingsData(new Dictionary<string, object>(), name);
            values.ReadAll((key, value) => source[key] = value);

            return source;
        }

        /// <summary>
        /// Can be used with dotted paths like:  AppSettings.Child.Nested.Property1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                var path = new DictionaryPath(key);
                return path.GetParentSource(this).Get(path.Key);
            } 
            set
            {
                var path = new DictionaryPath(key);
                _children[path].Set(path.Key, value);
            }
        }

        public string Provenance { get; set; }

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

        public SettingsData Child(string key)
        {
            if (!HasChild(key))
            {
                var dict = new Dictionary<string, object>();
                _dictionary.Add(key, dict);
            }

            var childDict = _dictionary.Child(key);
            return new SettingsData(childDict, Provenance + "." + key);
        }

        public bool HasChild(string key)
        {
            return Has(key)
                       ? _dictionary[key] is IDictionary<string, object> : false;
        }


        IValueSource IValueSource.GetChild(string key)
        {
            return Child(key);
        }

        IEnumerable<IValueSource> IValueSource.GetChildren(string key)
        {
            if (!Has(key)) return Enumerable.Empty<IValueSource>();

            var enumerable = _dictionary[key] as IEnumerable<IDictionary<string, object>>;
            if (enumerable == null)
            {
                return Enumerable.Empty<IValueSource>();
            }

            var i = 0;
            return enumerable.Select(x =>
            {
                var name = "{0}.{1}[{2}]".ToFormat(Provenance, key, i);

                i++;
                return new SettingsData(x, name);
            }).ToList();
        }

        public void WriteReport(IValueReport report)
        {
            _dictionary.Keys.ToList().Each(key =>
            {
                var value = _dictionary[key];

                if (value is IDictionary<string, object>)
                {
                    report.StartChild(key);
                    var child = Child(key);
                    child.WriteReport(report);
                    report.EndChild();
                }
                else if (value is IEnumerable<IDictionary<string, object>>)
                {
                    var children = value.As<IEnumerable<IDictionary<string, object>>>()
                        .Select(x => new SettingsData(x))
                        .ToList();


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

        bool IValueSource.Value(string key, Action<BindingValue> callback)
        {
            if (!Has(key)) return false;

            callback(new BindingValue(){
                RawKey = key,
                RawValue = _dictionary[key],
                Source = Provenance
            });

            return true;
        }

        public bool Equals(SettingsData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._dictionary, _dictionary) && Equals(other.Provenance, Provenance);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SettingsData)) return false;
            return Equals((SettingsData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dictionary != null ? _dictionary.GetHashCode() : 0)*397) ^ (Provenance != null ? Provenance.GetHashCode() : 0);
            }
        }

        public SettingsData GetChildrenElement(string key, int index)
        {
            IList<IDictionary<string, object>> list;

            if (!Has(key) || !(_dictionary[key] is IList<IDictionary<string, object>>))
            {
                list = new List<IDictionary<string, object>>();
                _dictionary.Add(key, list);
            }
            else
            {
                list = _dictionary[key] as IList<IDictionary<string, object>>;
            }

            while (index > list.Count - 1)
            {
                list.Add(new Dictionary<string, object>());
            }

            return new SettingsData(list[index]);
        }

        public IEnumerable<string> AllKeys
        {
            get
            {
                var report = new ValueDiagnosticReport();
                WriteReport(report);

                return report.AllValues().Select(x => x.Key);
            }
        }

        public void Read(string text)
        {
            StringPropertyReader.ReadLine(text, (key, value) => this[key] = value);
        }

        public static IEnumerable<SettingsData> Order(IEnumerable<SettingsData> settings)
        {
            var list = new List<SettingsData>();

            list.AddRange(settings.Where(x => x.Category == SettingCategory.profile));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.environment));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.package));
            list.AddRange(settings.Where(x => x.Category == SettingCategory.core));


            return list;
        }

        public static SettingsData ReadFromFile(SettingCategory category, string file)
        {
            var data = new SettingsData(category)
            {
                Provenance = file
            };

            StringPropertyReader.ForFile(file).ReadProperties((key, value) => data[key] = value);

            return data;
        }

        public static void ReadFromFile(string file, SettingsData data)
        {
            StringPropertyReader.ForFile(file).ReadProperties((key, value) => data[key] = value);
        }

        public SettingCategory Category { get; set; }

        public SettingsData With(string key, string value)
        {
            this[key] = value;
            return this;
        }


        public override string ToString()
        {
            return string.Format("ValueSource, category {0}, provenance: {1}", Category, Provenance ?? "unknown");
        }
    }


}