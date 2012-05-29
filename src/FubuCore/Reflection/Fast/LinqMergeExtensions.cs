using System.Linq;

namespace FubuCore.Reflection.Fast
{
    using System;
    using System.Collections.Generic;

    public static class LinqMergeExtensions
    {
        public static IEnumerable<TResult> Merge<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            Guard.AgainstNull(first, "first");
            Guard.AgainstNull(second, "second");
            Guard.AgainstNull(resultSelector, "resultSelector");

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
                while (e1.MoveNext())
                {
                    if (!e2.MoveNext())
                        yield break;

                    yield return resultSelector(e1.Current, e2.Current);
                }
        }


        public static IEnumerable<TResult> MergeBalanced<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            Guard.AgainstNull(first, "first");
            Guard.AgainstNull(second, "second");
            Guard.AgainstNull(resultSelector, "resultSelector");

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!e2.MoveNext())
                        throw new InvalidOperationException("Second sequence ran out before first");

                    yield return resultSelector(e1.Current, e2.Current);
                }
                if (e2.MoveNext())
                    throw new InvalidOperationException("First sequence ran out before second");
            }
        }

        public static IEnumerable<TResult> MergePadded<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            Guard.AgainstNull(first, "first");
            Guard.AgainstNull(second, "second");
            Guard.AgainstNull(resultSelector, "resultSelector");

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (e2.MoveNext())
                    {
                        yield return resultSelector(e1.Current, e2.Current);
                    }
                    else
                    {
                        do
                        {
                            yield return resultSelector(e1.Current, default(TSecond));
                        } while (e1.MoveNext());
                        yield break;
                    }
                }
                if (e2.MoveNext())
                {
                    do
                    {
                        yield return resultSelector(default(TFirst), e2.Current);
                    } while (e2.MoveNext());
                    yield break;
                }
            }
        }
    }

    public static class ExtensionsToGenerics
    {
        public static IEnumerable<Type> GetGenericTypeDeclarations(this object obj, Type genericType)
        {
            Guard.AgainstNull(obj, "obj");

            return obj.GetType().GetGenericTypeDeclarations(genericType);
        }

        public static IEnumerable<Type> GetGenericTypeDeclarations(this Type objectType, Type genericType)
        {
            Guard.AgainstNull(objectType, "objectType");
            Guard.AgainstNull(genericType, "genericType");
            Guard.IsTrue(x => x.IsGenericTypeDefinition, genericType, "genericType", "Must be an open generic type");

            Type matchedType;
            if (objectType.ImplementsGeneric(genericType, out matchedType))
            {
                foreach (Type argument in matchedType.GetGenericArguments())
                {
                    yield return argument;
                }
            }
        }
    }

    public static class ExtensionsToInterfaces
    {
        /// <summary>
        ///   Checks if an object implements the specified interface
        /// </summary>
        /// <typeparam name = "T">The interface type</typeparam>
        /// <param name = "obj">The object to check</param>
        /// <returns>True if the interface is implemented by the object, otherwise false</returns>
        public static bool Implements<T>(this object obj)
        {
            return obj.Implements(typeof(T));
        }

        /// <summary>
        ///   Checks if an object implements the specified interface
        /// </summary>
        /// <param name = "obj">The object to check</param>
        /// <param name = "interfaceType">The interface type (can be generic, either specific or open)</param>
        /// <returns>True if the interface is implemented by the object, otherwise false</returns>
        public static bool Implements(this object obj, Type interfaceType)
        {
            Guard.AgainstNull(obj, "obj");

            Type objectType = obj.GetType();

            return objectType.Implements(interfaceType);
        }

        /// <summary>
        ///   Checks if a type implements the specified interface
        /// </summary>
        /// <typeparam name = "T">The interface type (can be generic, either specific or open)</typeparam>
        /// <param name = "objectType">The type to check</param>
        /// <returns>True if the interface is implemented by the type, otherwise false</returns>
        public static bool Implements<T>(this Type objectType)
        {
            return objectType.Implements(typeof(T));
        }

        /// <summary>
        ///   Checks if a type implements the specified interface
        /// </summary>
        /// <param name = "objectType">The type to check</param>
        /// <param name = "interfaceType">The interface type (can be generic, either specific or open)</param>
        /// <returns>True if the interface is implemented by the type, otherwise false</returns>
        public static bool Implements(this Type objectType, Type interfaceType)
        {
            Guard.AgainstNull(objectType, "objectType");
            Guard.AgainstNull(interfaceType, "interfaceType");
            //			Guard.IsTrue(x => x.IsInterface, interfaceType, "interfaceType", "Must be an interface");

            if (interfaceType.IsGenericTypeDefinition)
                return objectType.ImplementsGeneric(interfaceType);

            return interfaceType.IsAssignableFrom(objectType);
        }

        /// <summary>
        ///   Checks if a type implements an open generic at any level of the inheritance chain, including all
        ///   base classes
        /// </summary>
        /// <param name = "objectType">The type to check</param>
        /// <param name = "interfaceType">The interface type (must be a generic type definition)</param>
        /// <returns>True if the interface is implemented by the type, otherwise false</returns>
        public static bool ImplementsGeneric(this Type objectType, Type interfaceType)
        {
            Type matchedType;
            return objectType.ImplementsGeneric(interfaceType, out matchedType);
        }

        /// <summary>
        ///   Checks if a type implements an open generic at any level of the inheritance chain, including all
        ///   base classes
        /// </summary>
        /// <param name = "objectType">The type to check</param>
        /// <param name = "interfaceType">The interface type (must be a generic type definition)</param>
        /// <param name = "matchedType">The matching type that was found for the interface type</param>
        /// <returns>True if the interface is implemented by the type, otherwise false</returns>
        public static bool ImplementsGeneric(this Type objectType, Type interfaceType, out Type matchedType)
        {
            Guard.AgainstNull(objectType);
            Guard.AgainstNull(interfaceType);
            Guard.IsTrue(x => x.IsGenericType, interfaceType, "interfaceType", "Must be a generic type");
            Guard.IsTrue(x => x.IsGenericTypeDefinition, interfaceType, "interfaceType", "Must be a generic type definition");

            matchedType = null;

            if (interfaceType.IsInterface)
            {
                matchedType = objectType.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType)
                    .FirstOrDefault();
                if (matchedType != null)
                    return true;
            }

            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == interfaceType)
            {
                matchedType = objectType;
                return true;
            }

            Type baseType = objectType.BaseType;
            if (baseType == null)
                return false;

            return baseType.ImplementsGeneric(interfaceType, out matchedType);
        }
    }
}