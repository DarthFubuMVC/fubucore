using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuTestingSupport
{
    public class EnumerablePersistenceCheck<T> : AccessorPersistenceCheck
    {
        public static EnumerablePersistenceCheck<T> For<TParent>(Expression<Func<TParent, IEnumerable<T>>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            return new EnumerablePersistenceCheck<T>(accessor);
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