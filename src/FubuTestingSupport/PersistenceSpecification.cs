using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using System.Linq;
using NUnit.Framework;
using FubuCore;

namespace FubuTestingSupport
{
    

    public class PersistenceSpecification<T>
    {
        private readonly Func<T, T> _persistor;
        private readonly IList<IPersistenceCheck> _checks = new List<IPersistenceCheck>();

        public PersistenceSpecification(Func<T, T> persistor)
        {
            _persistor = persistor;
        }

        public T Original { get; set; }

        public void Check(params Expression<Func<T, object>>[] expressions)
        {
            expressions.Each(x =>
            {
                var check = new AccessorPersistenceCheck(x.ToAccessor());
                _checks.Add(check);
            });
        }

        public void Verify()
        {
            if (Original == null)
            {
                throw new ArgumentNullException("No original data is specified");
            }

            var messages = new List<string>();

            var persisted = _persistor(Original);

            _checks.Each(x => x.CheckValue(Original, persisted, messages.Add));

            if (messages.Any())
            {
                var message = "Persistence Specification failed:\n" + messages.Join("\n");
                Assert.Fail(message);
            }
        }
    }


    public interface IPersistenceCheck
    {
        void CheckValue(object original, object persisted, Action<string> writeError);
    }

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

            if (originalValue == null)
            {
                if (persistedValue == null) return;
            }
            else
            {
                if (originalValue.Equals(persistedValue)) return;
            }

            var message = "{0}:  original was {1}, but the persisted value was {2}".ToFormat(_accessor.Name,
                                                                                             displayFor(originalValue),
                                                                                             displayFor(persistedValue));
            writeError(message);
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

            return value.ToString();
        }
    }
}