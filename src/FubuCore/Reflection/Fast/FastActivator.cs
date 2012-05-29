using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace FubuCore.Reflection.Fast
{
    public class FastActivator
	{
		[ThreadStatic]
		static FastActivator _current;

		readonly Dictionary<Type, IFastActivator> _generators;

		FastActivator()
		{
			_generators = new Dictionary<Type, IFastActivator>();
		}

		public static FastActivator Current
		{
			get
			{
				if (_current == null)
					_current = new FastActivator();

				return _current;
			}
		}

		IFastActivator GetGenerator(Type type)
		{
			return _generators.Retrieve(type, () =>
				{
					const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

					return (IFastActivator)typeof(FastActivator<>).MakeGenericType(type)
					                       	.GetProperty("Current", flags)
					                       	.GetValue(null, flags, null, null, CultureInfo.InvariantCulture);
				});
		}

		IFastActivator GetGenericGenerator(Type type)
		{
			return _generators.Retrieve(type, () => new GenericFastActivator(type));
		}

		public static object Create(Type type)
		{
			return Current.GetGenerator(type).Create();
		}

		public static object Create<TArg0>(Type type, TArg0 arg0)
		{
			if (type.IsGenericType)
				Current.GetGenericGenerator(type).Create(arg0);

			return Current.GetGenerator(type).Create(arg0);
		}

		public static object Create<TArg0, TArg1>(Type type, TArg0 arg0, TArg1 arg1)
		{
			if (type.IsGenericType)
				Current.GetGenericGenerator(type).Create(arg0, arg1);

			return Current.GetGenerator(type).Create(arg0, arg1);
		}

		public static object Create(Type type, object[] args)
		{
			if (type.IsGenericType)
				Current.GetGenericGenerator(type).Create(args);

			return Current.GetGenerator(type).Create(args);
		}

		public static object Create(Type type, Type[] genericTypes)
		{
			Type genericType = GetGenericType(type, genericTypes);

			return Current.GetGenerator(genericType).Create();
		}

		static Type GetGenericType(Type type, Type[] genericTypes)
		{
			if (!type.IsGenericType)
				throw new ArgumentException("The type specified must be a generic type");

			Type[] genericArguments = type.GetGenericArguments();

			if (genericArguments.Length != genericTypes.Length)
			{
				throw new ArgumentException("An incorrect number of generic arguments was specified: " + genericTypes.Length
				                            + " (needed " + genericArguments.Length + ")");
			}

			return type.MakeGenericType(genericTypes);
		}

		public static object Create(Type type, Type[] genericTypes, object[] args)
		{
			Type genericType = GetGenericType(type, genericTypes);

			return Current.GetGenerator(genericType).Create(args);
		}
	}
}