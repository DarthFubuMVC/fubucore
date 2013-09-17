using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection
{
    public class IndexerValueGetter : IValueGetter
    {
        private readonly PropertyInfo _arrayPropertyInfo;
        private readonly int _index;

        public IndexerValueGetter(PropertyInfo arrayPropertyInfo, int index)
        {
            _arrayPropertyInfo = arrayPropertyInfo;
            _index = index;
        }

        public object GetValue(object target)
        {
            return ((Array)target).GetValue(_index);
        }

        public string Name
        {
            get
            {
                return "[{0}]".ToFormat(_index);
            }
        }

        public Type DeclaringType
        {
            get { return _arrayPropertyInfo.DeclaringType; }
        }

        public Type ValueType
        {
            get { return _arrayPropertyInfo.PropertyType.GetElementType(); }
        }

        public Expression ChainExpression(Expression body)
        {
            var memberExpression = Expression.ArrayIndex(body, Expression.Constant(_index, typeof(int)));
            if (!_arrayPropertyInfo.PropertyType.GetElementType().IsValueType)
            {
                return memberExpression;
            }

            return Expression.Convert(memberExpression, typeof(object));
        }

        public void SetValue(object target, object propertyValue)
        {
            ((Array)target).SetValue(propertyValue, _index);
        }

        protected bool Equals(IndexerValueGetter other)
        {
            return Equals(_arrayPropertyInfo, other._arrayPropertyInfo) && _index == other._index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexerValueGetter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_arrayPropertyInfo != null ? _arrayPropertyInfo.GetHashCode() : 0)*397) ^ _index;
            }
        }
    }



}