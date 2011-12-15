using System;
using FubuCore.Reflection.Expressions;
using FubuCore.Util;

namespace FubuCore.Conversion
{
    public class StringConstructorConverterFamily : IObjectConverterFamily
    {
        public bool Matches(Type type, ConverterLibrary converter)
        {
            if (type.IsArray) return false;

            var constructorInfo = type.GetConstructor(new Type[]{typeof (string)});
            return constructorInfo != null;
        }

        public IConverterStrategy CreateConverter(Type type, Func<Type, IConverterStrategy> converterSource)
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