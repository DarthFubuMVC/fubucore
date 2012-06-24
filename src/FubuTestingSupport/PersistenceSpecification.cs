using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using System.Linq;
using NUnit.Framework;
using FubuCore;

namespace FubuTestingSupport
{
    public interface IPersistenceSpecification<T>
    {
        T Original { get; set; }
        void Check(params Expression<Func<T, object>>[] expressions);
        void CheckProperties(Func<PropertyInfo, bool> filter);
        void CheckAllPropertiesDeclaredBy<TDeclaringType>();
    }

    public class PersistenceSpecification<T> : IPersistenceSpecification<T>
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
                var accessor = x.ToAccessor();
                addAccessor(accessor);
            });
        }

        public void CheckProperties(Func<PropertyInfo, bool> filter)
        {
            typeof (T).GetProperties().Where(filter).Each(prop =>
            {
                var accessor = new SingleProperty(prop);
                addAccessor(accessor);
            });
        }

        public static IPersistenceCheck BuildCheck(Expression<Func<T, object>> expression)
        {
            return BuildCheck(expression.ToAccessor());
        }

        public static IPersistenceCheck BuildCheck(Accessor accessor)
        {
            var enumerableType = accessor.PropertyType.FindInterfaceThatCloses(typeof (IEnumerable<>));
            if (enumerableType != null)
            {
                var elementType = enumerableType.GetGenericArguments().Single();
                return typeof (EnumerablePersistenceCheck<>)
                    .CloseAndBuildAs<IPersistenceCheck>(accessor, elementType);
            }

            return new AccessorPersistenceCheck(accessor);
        }

        private void addAccessor(Accessor accessor)
        {
            var check = BuildCheck(accessor);
            _checks.Add(check);
        }

        public void CheckAllPropertiesDeclaredBy<TDeclaringType>()
        {
            CheckProperties(prop => prop.DeclaringType.Equals(typeof(TDeclaringType)));
        }

        

        public void Verify()
        {
            if (Original == null)
            {
                throw new ArgumentNullException("No original data is specified");
            }

            List<string> messages = GetMessages();

            if (messages.Any())
            {
                var message = "Persistence Specification failed:\n" + messages.Join("\n");
                Assert.Fail(message);
            }
        }

        public List<string> GetMessages()
        {
            var messages = new List<string>();

            var persisted = _persistor(Original);

            _checks.Each(x => x.CheckValue(Original, persisted, messages.Add));
            return messages;
        }
    }
}