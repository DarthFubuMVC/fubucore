using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using System.Linq;
using NUnit.Framework;

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

    public class EnumerablePersistenceCheck<T> : AccessorPersistenceCheck
    {
        public new static EnumerablePersistenceCheck<TElement> For<T, TElement>(Expression<Func<T, IEnumerable<TElement>>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            return new EnumerablePersistenceCheck<TElement>(accessor);
        }

        public EnumerablePersistenceCheck(Accessor accessor) : base(accessor)
        {
        }

        protected override bool matches(object originalValue, object persistedValue)
        {
            IEnumerable<T> enum1 = originalValue as IEnumerable<T>;
            IEnumerable<T> enum2 = persistedValue as IEnumerable<T>;

            if (enum1 == null)
            {
                if (enum2 == null) return true;
            }
            else if (enum2 == null) return false;
            else
            {
                if (enum1.SequenceEqual(enum2)) return true;
            }

            return false;
        }
    }
}