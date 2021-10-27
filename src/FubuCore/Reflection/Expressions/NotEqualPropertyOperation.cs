using FubuCore.Util;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection.Expressions
{
    public class NotEqualPropertyOperation : BinaryComparisonPropertyOperation
    {
        static readonly Cache<Type, MethodInfo> InequalityMethodCache = new Cache<Type, MethodInfo>(LookupInequalityMethod);

        public NotEqualPropertyOperation()
            : base(ExpressionType.NotEqual)
        {
        }

        public override MethodInfo Method(object expected) =>
            expected != null ? InequalityMethodCache[expected.GetType()] : null;

        static MethodInfo LookupInequalityMethod(Type type)
        {
            if (type == typeof(object) || type.IsValueType)
            {
                return null;
            }

            var method = type.GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                return method;
            }

            return LookupInequalityMethod(type.BaseType);
        }

        public override string OperationName { get { return "IsNot"; } }
        public override string Text
        {
            get { return "is not"; }
        }
    }
}