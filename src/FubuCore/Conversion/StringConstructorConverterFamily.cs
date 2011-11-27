using System;
using FubuCore.Reflection.Expressions;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class StringConstructorConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, IObjectConverter converter)
        {
            if (type.IsArray) return false;

            var constructorInfo = type.GetConstructor(new Type[]{typeof (string)});
            return constructorInfo != null;
        }

        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            var builder = typeof (FuncBuilder<>).CloseAndBuildAs<FuncBuilder>(type);
            return builder.Build;
        }

        public interface FuncBuilder
        {
            object Build(string value);
        }

        public class FuncBuilder<T> : FuncBuilder
        {
            private Func<string, T> _func;

            public FuncBuilder()
            {
                _func = ConstructorBuilder.CreateSingleStringArgumentConstructor(typeof (T))
                    .Compile().As<Func<string, T>>();
            }

            public object Build(string value)
            {
                return _func(value);
            }
        }
    }
}