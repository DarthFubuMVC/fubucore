using FubuCore.Util;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection.Expressions
{
    public class EqualsPropertyOperation : BinaryComparisonPropertyOperation
    {
        static readonly Cache<Type, MethodInfo> EqualityMethodCache = new Cache<Type, MethodInfo>(LookupEqualityMethod);
        
        public EqualsPropertyOperation()
            : base(ExpressionType.Equal)
        {
        }

        public override MethodInfo Method(object expected) =>
            expected != null ? EqualityMethodCache[expected.GetType()] : null;

        static MethodInfo LookupEqualityMethod(Type type)
        {
            if (type == typeof(object) || type.IsValueType)
            {
                return null;
            }

            var method = type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                return method;
            }

            return LookupEqualityMethod(type.BaseType);
        }

        public override string OperationName { get { return "Is"; } }
        public override string Text
        {
            get { return "is"; }
        }
    }
}