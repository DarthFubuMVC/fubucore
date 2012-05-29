// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection.Fast
{
    public static class TypeSpecializationExtensions
	{
		public static Type ToSpecializedType<T>(this T method, object[] args)
			where T : MethodBase
		{
			Guard.AgainstNull(method, "method");

			Type type = method.DeclaringType;
			if (!type.IsGenericType)
				throw new ArgumentException("The argument must be for a generic type", "method");

			Guard.AgainstNull(args, "args");

			Type[] genericArguments = GetGenericTypesFromArguments(method.GetParameters(), type.GetGenericArguments(), args);

			return type.MakeGenericType(genericArguments);
		}

		public static MethodInfo ToSpecializedMethod(this MethodInfo method, object[] args)
		{
			Guard.AgainstNull(method, "method");

			if (!method.IsGenericMethod)
				return method;

			Guard.AgainstNull(args, "args");

			Type[] genericArguments = GetGenericTypesFromArguments(method.GetParameters(), method.GetGenericArguments(), args);

			return method.MakeGenericMethod(genericArguments);
		}

		public static MethodInfo ToSpecializedMethod(this MethodInfo method, Type[] genericTypes, object[] args)
		{
			Guard.AgainstNull(method, "method");

			if (!method.IsGenericMethod)
				return method;

			Guard.AgainstNull(genericTypes, "genericTypes");
			Guard.AgainstNull(args, "args");

			Type[] arguments = method.GetGenericArguments()
				.ApplyGenericTypesToArguments(genericTypes);

			arguments = GetGenericTypesFromArguments(method.GetParameters(), arguments, args);

			method = method.MakeGenericMethod(arguments);

			return method;
		}

		private static Type[] ApplyGenericTypesToArguments(this Type[] arguments, Type[] genericTypes)
		{
			for (int i = 0; i < arguments.Length && i < genericTypes.Length; i++)
			{
				if (genericTypes[i] != null)
					arguments[i] = genericTypes[i];
			}

			return arguments;
		}

		private static Type[] GetGenericTypesFromArguments(ParameterInfo[] parameterInfos, Type[] arguments, object[] args)
		{
			var parameters = parameterInfos
				.Merge(args, (x, y) => new {Parameter = x, Argument = y});

			for (int i = 0; i < arguments.Length; i++)
			{
				Type argumentType = arguments[i];

				if (!argumentType.IsGenericParameter)
					continue;

				parameters
					.Where(arg => arg.Parameter.ParameterType == argumentType && arg.Argument != null)
					.Select(arg => arg.Argument.GetType())
					.Each(type =>
						{
							arguments[i] = type;

							var more = argumentType.GetGenericParameterConstraints()
								.Where(x => x.IsGenericType)
								.Where(x => type.Implements(x.GetGenericTypeDefinition()))
								.SelectMany(x => x.GetGenericArguments()
													.Merge(type.GetGenericTypeDeclarations(x.GetGenericTypeDefinition()), (c, a) => new { Argument = c, Type = a }));

							foreach (var next in more)
							{
								for (int j = 0; j < arguments.Length; j++)
								{
									if (arguments[j] == next.Argument)
										arguments[j] = next.Type;
								}
							}
						});

				foreach (var parameter in parameters.Where(x => x.Parameter.ParameterType.IsGenericType && x.Argument != null))
				{
					var definition = parameter.Parameter.ParameterType.GetGenericTypeDefinition();
					var declaredTypesForGeneric = parameter.Argument.GetType().GetGenericTypeDeclarations(definition);

					var mergeds = parameter.Parameter.ParameterType.GetGenericArguments()
						.Merge(declaredTypesForGeneric, (p, a) => new { ParameterType = p, ArgumentType = a });

					foreach (var merged in mergeds)
					{
						for (int j = 0; j < arguments.Length; j++)
						{
							if (arguments[j] == merged.ParameterType)
								arguments[j] = merged.ArgumentType;
						}
					}
				}
			}

			return arguments;
		}
	}

    public static class ExtensionsToExpression
    {
        /// <summary>
        /// Gets the name of the member specified
        /// </summary>
        /// <typeparam name="T">The type referenced</typeparam>
        /// <typeparam name="TMember">The type of the member referenced</typeparam>
        /// <param name="expression">The expression referencing the member</param>
        /// <returns>The name of the member referenced by the expression</returns>
        public static string MemberName<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        /// <summary>
        /// Gets the name of the member specified
        /// </summary>
        /// <typeparam name="T">The type referenced</typeparam>
        /// <param name="expression">The expression referencing the member</param>
        /// <returns>The name of the member referenced by the expression</returns>
        public static string MemberName<T>(this Expression<Action<T>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        /// <summary>
        /// Gets the name of the member specified
        /// </summary>
        /// <typeparam name="T1">The type referenced</typeparam>
        /// <typeparam name="T2">The type of the member referenced</typeparam>
        /// <param name="expression">The expression referencing the member</param>
        /// <returns>The name of the member referenced by the expression</returns>
        public static string MemberName<T1, T2>(this Expression<Action<T1, T2>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        public static PropertyInfo GetMemberPropertyInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            return expression.GetMemberExpression().Member as PropertyInfo;
        }

        public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            Guard.AgainstNull(expression, "expression");

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T>(this Expression<Action<T>> expression)
        {
            Guard.AgainstNull(expression, "expression");

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T1, T2>(this Expression<Action<T1, T2>> expression)
        {
            Guard.AgainstNull(expression, "expression");

            return GetMemberExpression(expression.Body);
        }

        /// <summary>
        /// Wraps an action expression with no arguments inside an expression that takes an 
        /// argument of the specified type (the argument is ignored, but the original expression is
        /// invoked)
        /// </summary>
        /// <typeparam name="TArgument">The type of argument to accept in the wrapping expression</typeparam>
        /// <param name="expression">The expression to wrap</param>
        /// <returns></returns>
        public static Expression<Action<TArgument>> WrapActionWithArgument<TArgument>(this Expression<Action> expression)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TArgument), "x");

            return Expression.Lambda<Action<TArgument>>(Expression.Invoke(expression), parameter);
        }

        static MemberExpression GetMemberExpression(Expression body)
        {
            Guard.AgainstNull(body, "body");

            MemberExpression memberExpression = null;
            if (body.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = (UnaryExpression)body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
                memberExpression = body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("Expression is not a member access");

            return memberExpression;
        }


        public static Expression<Func<T1, TResult>> Curry<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> expression,
            T2 value)
        {
            return new CurryExpressionVisitor<T1, T2, TResult>().Curry(expression, value);
        }
    }
}