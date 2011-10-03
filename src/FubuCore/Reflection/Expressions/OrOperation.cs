using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace FubuCore.Reflection.Expressions
{
    public class ComposableOrOperation
    {
        private List<Tuple<IPropertyOperation, MemberExpression, object>> _listOfOperations = new List<Tuple<IPropertyOperation, MemberExpression, object>>();

        public void Set<T>(Expression<Func<T, object>> path, object value)
        {
            var memberExpression = path.GetMemberExpression(true);
            var operation = new EqualsPropertyOperation();
            _listOfOperations.Add(new Tuple<IPropertyOperation, MemberExpression, object>(operation, memberExpression, value));
        }


        public void Set<T>(Expression<Func<T, object>> path, IEnumerable<object> value)
        {
            var memberExpression = path.GetMemberExpression(true);
            var operation = new CollectionContainsPropertyOperation();
            _listOfOperations.Add(new Tuple<IPropertyOperation, MemberExpression, object>(operation, memberExpression, value));
        }



        public Expression<Func<T, bool>> GetPredicateBuilder<T>()
        {
            if(_listOfOperations.Count() > 2)
            {
                throw new Exception("You can't have more than two operations registered for an 'or' operation (you have {0})".ToFormat(_listOfOperations.Count));
            }

            //the parameter to use
            var lambdaParameter = Expression.Parameter(typeof (T));


            //make predicates
            var leftOptions = _listOfOperations.First();
            var leftPredicateBuilder = leftOptions.Item1.GetPredicateBuilder<T>(leftOptions.Item2);
            var leftPredicate = leftPredicateBuilder(leftOptions.Item3);

            var rightOptions = _listOfOperations.Skip(1).First();
            var rightPredicateBuilder = rightOptions.Item1.GetPredicateBuilder<T>(rightOptions.Item2);
            var rightPredicate = rightPredicateBuilder(rightOptions.Item3);

            

            //to avoid invokes and calls I need to rebuild the predicates with OUR parameter
            var lb = rebuild(leftPredicate, lambdaParameter);
            var rb = rebuild(rightPredicate, lambdaParameter);

            var orElse = Expression.OrElse(lb, rb);
            var expressionToReturn = Expression.Lambda<Func<T, bool>>(orElse, lambdaParameter);

            return expressionToReturn;
        }

        Expression rebuild(Expression exp, ParameterExpression parameter)
        {
            var lb = (LambdaExpression) exp;
            var targetBody = lb.Body;
            if(targetBody.NodeType == ExpressionType.Equal)
            {
                return rebuildBinary((BinaryExpression)targetBody, parameter);
            }
            else if(targetBody.NodeType == ExpressionType.Call)
            {
                return rebuildMethodCall((MethodCallExpression)targetBody, parameter);
            }
            else
            {
                return null;
            }
        }

        Expression rebuildMemberExpression(MemberExpression mem, ParameterExpression param)
        {
            return Expression.MakeMemberAccess(param ,mem.Member);
        }
        Expression rebuildBinary(BinaryExpression exp, ParameterExpression parameter)
        {
            var a = rebuildMemberExpression((MemberExpression)exp.Left, parameter);
            return Expression.Equal(a, exp.Right);
        }
        Expression rebuildMethodCall(MethodCallExpression exp, ParameterExpression parameter)
        {
            //currently only works with extension methods
            var args = new[] {exp.Arguments.First(), parameter };

            return Expression.Call(exp.Method, args);
        }

    }
    public class OrOperation
    {
        
        public Expression<Func<T, bool>> GetPredicateBuilder<T>(Expression<Func<T, object>> leftPath, object leftValue, Expression<Func<T, object>> rightPath, object rightValue)
        {
            var comp = new ComposableOrOperation();
            comp.Set(leftPath, leftValue);
            comp.Set(rightPath, rightValue);
            return comp.GetPredicateBuilder<T>();
        }


        public Expression<Func<T, bool>> GetPredicateBuilder<T>(Expression<Func<T, object>> leftPath, object leftValue, Expression<Func<T, object>> rightPath, IEnumerable<object> rightValue)
        {
            var comp = new ComposableOrOperation();
            comp.Set(leftPath, leftValue);
            comp.Set(rightPath, rightValue);
            return comp.GetPredicateBuilder<T>();
        }
    }
}