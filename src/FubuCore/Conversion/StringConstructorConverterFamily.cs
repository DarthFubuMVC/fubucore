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

        public IConverterStrategy CreateConverter(Type type, Cache<Type, IConverterStrategy> converters)
        {
            return typeof(FuncBuilder<>).CloseAndBuildAs<IConverterStrategy>(type);
        }

        public class FuncBuilder<T> : IConverterStrategy
        {
            private readonly Func<string, T> _func;

            public FuncBuilder()
            {
                _func = ConstructorBuilder.CreateSingleStringArgumentConstructor(typeof (T))
                    .Compile().As<Func<string, T>>();
            }

            public object Convert(IConversionRequest request)
            {
                return _func(request.Text);
            }
        }

        
    }
}