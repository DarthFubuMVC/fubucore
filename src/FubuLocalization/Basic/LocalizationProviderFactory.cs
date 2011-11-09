using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FubuCore;

namespace FubuLocalization.Basic
{
    public interface ILocalizationCache
    {
        ILocaleCache CacheFor(CultureInfo culture, Func<IEnumerable<LocalString>> finder);
        void LoadCaches(Action<Action<CultureInfo, ILocaleCache>> loader);
        void Clear();
    }

    public class LocalizationCache : ILocalizationCache
    {
        private readonly Dictionary<string, ILocaleCache> _locales = new Dictionary<string, ILocaleCache>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ILocaleCache CacheFor(CultureInfo culture, Func<IEnumerable<LocalString>> finder)
        {
            var cache = initialRead(culture);
            if (cache == null)
            {
                _lock.Write(() =>
                {
                    if (!_locales.ContainsKey(culture.Name))
                    {
                        var data = finder();
                        cache = new ThreadSafeLocaleCache(culture, data);
                        _locales.Add(culture.Name, cache);
                    }
                    else
                    {
                        cache = _locales[culture.Name];
                    }
                });
            }

            return cache;
        }

        public void LoadCaches(Action<Action<CultureInfo, ILocaleCache>> loader)
        {
            _lock.Write(() =>
            {
                _locales.Clear();

                loader((culture, cache) => _locales.Add(culture.Name, cache));
            });
        }

        private ILocaleCache initialRead(CultureInfo culture)
        {
            return _lock.Read(() => _locales.ContainsKey(culture.Name) ? _locales[culture.Name] : null);
        }

        public void Clear()
        {
            _lock.Write(() => _locales.Clear());
        }

    }

    public class LocalizationProviderFactory
    {
        private readonly ILocalizationStorage _storage;
        private readonly ILocalizationMissingHandler _missingHandler;
        private readonly ILocalizationCache _cache;


        public LocalizationProviderFactory(ILocalizationStorage storage, ILocalizationMissingHandler missingHandler, ILocalizationCache cache)
        {
            _storage = storage;
            _missingHandler = missingHandler;
            _cache = cache;
        }

        public void LoadAll()
        {
            _cache.LoadCaches(loader =>
            {
                _storage.LoadAll((c, keys) =>
                {
                    loader(c, new ThreadSafeLocaleCache(c, keys));
                });
            });
        }

        public ILocalizationDataProvider SelectProvider(CultureInfo culture)
        {
            return new LocalizationProvider(_cache.CacheFor(culture, () => _storage.Load(culture)), _missingHandler);
        }
    }
}