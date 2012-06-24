using System;
using System.Collections;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using System.Linq;

namespace FubuTestingSupport
{
    public class AccessorPersistenceCheck : IPersistenceCheck
    {
        private readonly Accessor _accessor;

        public static AccessorPersistenceCheck For<T>(Expression<Func<T, object>> expression)
        {
            return new AccessorPersistenceCheck(expression.ToAccessor());
        }

        public AccessorPersistenceCheck(Accessor accessor)
        {
            _accessor = accessor;
        }

        public void CheckValue(object original, object persisted, Action<string> writeError)
        {
            var originalValue = _accessor.GetValue(original);
            var persistedValue = _accessor.GetValue(persisted);

            if (matches(originalValue, persistedValue)) return;

            string message = writeMessage(originalValue, persistedValue);
            writeError(message);
        }

        protected virtual string writeMessage(object originalValue, object persistedValue)
        {
            return "{0}:  original was {1}, but the persisted value was {2}".ToFormat(_accessor.Name,
                                                                                      displayFor(originalValue),
                                                                                      displayFor(persistedValue));
        }

        protected virtual bool matches(object originalValue, object persistedValue)
        {
            if (originalValue == null)
            {
                if (persistedValue == null) return true;
            }
            else
            {
                if (originalValue.Equals(persistedValue)) return true;
            }

            return false;
        }

        private static string displayFor(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string)
            {
                return "'" + value.ToString() + "'";
            }

            if (value is IEnumerable)
            {
                return string.Join(", ", value.As<IEnumerable>().OfType<object>().ToArray());
            }

            return value.ToString();
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }
    }
}