using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FubuCore.Reflection.Expressions
{
    public class OrOperation
    {
        public Expression<Func<T, bool>> GetPredicateBuilder<T>(Expression<Func<T, object>> leftPath, object leftValue, Expression<Func<T, object>> rightPath, object rightValue)
        {
            var left = leftPath.GetMemberExpression(true);
            var right = rightPath.GetMemberExpression(true);

            var leftGuard = new EqualsPropertyOperation().GetPredicateBuilder<T>(left);
            var rightGuard = new EqualsPropertyOperation().GetPredicateBuilder<T>(right);

            return valueToTest => leftGuard(leftValue).Compile()(valueToTest) || rightGuard(rightValue).Compile()(valueToTest);
        }


        public Expression<Func<T, bool>> GetPredicateBuilder<T>(Expression<Func<T, object>> leftPath, object leftValue, Expression<Func<T, object>> rightPath, IEnumerable<object> rightValue)
        {
            var left = leftPath.GetMemberExpression(true);
            var right = rightPath.GetMemberExpression(true);

            var leftGuard = new EqualsPropertyOperation().GetPredicateBuilder<T>(left);

            //use brando's class here
            var rightGuard = new CollectionContainsPropertyOperation().GetPredicateBuilder<T>(right);

            return valueToTest => leftGuard(leftValue).Compile()(valueToTest) || rightGuard(rightValue).Compile()(valueToTest);
        }
    }
}