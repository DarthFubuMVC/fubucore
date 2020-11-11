using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using Moq.AutoMock;

namespace FubuCore.Testing
{
    public static class MoqExtensions
    {
        public static void VerifyNotCalled<TResult, T>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.Never);
        }

        public static void VerifyNotCalled<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Never);
        }

        public static void Inject<T>(this AutoMocker mocker, T value)
        {
            mocker.Use(value);
        }
    }

    public class Arg<T>
    {
        public static ArgExpression<T> Is => new ArgExpression<T>();
    }

    public class ArgExpression<T>
    {
        public T Equal(T value)
        {
            return It.Is(value, EqualityComparer<T>.Default);
        }

        public T Anything => It.IsAny<T>();
    }
}