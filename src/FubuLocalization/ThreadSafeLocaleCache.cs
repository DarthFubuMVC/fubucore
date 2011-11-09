using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FubuCore;

namespace FubuLocalization
{
    public class ThreadSafeLocaleCache : ILocaleCache
    {
        private readonly CultureInfo _culture;
        private readonly IDictionary<LocalizationKey, string> _data;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ThreadSafeLocaleCache(CultureInfo culture, IEnumerable<LocalString> strings)
        {
            _data = new Dictionary<LocalizationKey, string>();
            strings.Each(s => _data.Add(new LocalizationKey(s.value), s.display));

            _culture = culture;
        }

        public ThreadSafeLocaleCache(CultureInfo culture, IDictionary<LocalizationKey, string> data)
        {
            _culture = culture;
            _data = data;
        }

        public CultureInfo Culture { get { return _culture; } }

        public void Append(LocalizationKey key, string value)
        {
            _lock.Write(() =>
            {
                if (_data.ContainsKey(key))
                {
                    _data[key] = value;
                }
                else
                {
                    _data.Add(key, value);
                }
            });
        }

        public string Retrieve(LocalizationKey key, Func<string> missing)
        {
            var text = initialRead(key);

            if (text == null)
            {
                text = missing();
                Append(key, text);
            }

            return text;
        }

        private string initialRead(LocalizationKey key)
        {
            return _lock.Read(() => !_data.ContainsKey(key) ? null : _data[key]);
        }
    }
}