using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;
using System.Linq;

namespace FubuLocalization
{

    public class StringToken
    {
        private readonly string _defaultValue;
        private readonly string _localizationNamespace;
        private string _key;
        private readonly Lazy<LocalizationKey> _localizationKey;

        private static readonly IList<Type> _latchedTypes = new List<Type>();

        protected static void fillKeysOnFields(Type tokenType)
        {
            if (_latchedTypes.Contains(tokenType)) return;

            tokenType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.FieldType.CanBeCastTo<StringToken>())
                .Each(field =>
                {
                    var token = field.GetValue(null).As<StringToken>();
                    if (token._key == null) // leave it checking the field, unless you just really enjoy stack overflow exceptions
                    {
                        token.Key = field.Name;
                    }
                });
                

            _latchedTypes.Add(tokenType);
        }

        protected StringToken(string key, string defaultValue, string localizationNamespace = null, bool namespaceByType = false)
        {
            _key = key;
            _defaultValue = defaultValue;
            _localizationNamespace = localizationNamespace ?? (namespaceByType ? GetType().Name : null);

            _localizationKey = new Lazy<LocalizationKey>(buildKey);
        }

        public string Key
        {
            get
            {
                if (_key.IsEmpty())
                {
                    fillKeysOnFields(GetType());
                }
                
                return _key;
            }
            protected set { _key = value; }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
        }

        public static StringToken FromKeyString(string key)
        {
            return new StringToken(key, null);
        }

        public static StringToken FromKeyString(string key, string defaultValue)
        {
            return new StringToken(key, defaultValue);
        }

        public static StringToken FromType<T>()
        {
            return FromType(typeof (T));
        }

        public static StringToken FromType(Type type)
        {
            return new StringToken(type.Name, type.Name);
        }

        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        ///   Conditionally render the string based on a condition. Convenient if you want to avoid a bunch of messy script tags in the views.
        /// </summary>
        /// <param name = "condition"></param>
        /// <returns></returns>
        public string ToString(bool condition)
        {
            return condition ? LocalizationManager.GetTextForKey(this) : string.Empty;
        }

        public string ToFormat(params object[] args)
        {
            return string.Format(ToString(), args);
        }

        public bool Equals(StringToken obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._key, _key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StringToken)) return false;
            return Equals((StringToken) obj);
        }

        public override int GetHashCode()
        {
            return (_key != null ? _key.GetHashCode() : 0);
        }


        protected LocalizationKey buildKey()
        {
            if (_key == null)
            {
                fillKeysOnFields(GetType());
            }

            return _localizationNamespace.IsNotEmpty() 
                ? new LocalizationKey(_localizationNamespace + ":" + _key) 
                : new LocalizationKey(_key);
        }

        public LocalizationKey ToLocalizationKey()
        {
            return _localizationKey.Value;
        }
    }
}