using System;
using FubuMVC.Core;


namespace FubuCore.Binding
{
    public class ObjectResolver : IObjectResolver
    {
        private readonly IModelBinderCache _binders;
        private readonly IBindingLogger _logger;
        private readonly IServiceLocator _services;

        public ObjectResolver(IServiceLocator services, IModelBinderCache binders, IBindingLogger logger)
        {
            _services = services;
            _binders = binders;
            _logger = logger;
        }

        public virtual BindResult BindModel(Type type, IRequestData data)
        {
            var context = new BindingContext(data, _services, _logger);
            return BindModel(type, context);
        }

        public virtual BindResult BindModel<T>(T model, IBindingContext context)
        {
            var binder = findBinder(context, typeof(T));
            return executeModelBinder(typeof (T), context, () =>
            {
                binder.Bind(typeof (T), model, context);
                return new BindResult{
                    Problems = context.Problems,
                    Value = model
                };
            });
        }

        // Leave this virtual
        public virtual BindResult BindModel(Type type, IBindingContext context)
        {
            var binder = findBinder(context, type);
            return executeModelBinder(type, context, () => binder.Bind(type, context));
        }

        private IModelBinder findBinder(IBindingContext context, Type type)
        {
            var binder = _binders.BinderFor(type);

            if (binder == null)
            {
                throw new FubuException(2200,
                                        "Could not determine an IModelBinder for input type {0}. No model binders matched on this type. The standard model binder requires a parameterless constructor for the model type. Alternatively, you could implement your own IModelBinder which can process this model type.",
                                        type.AssemblyQualifiedName);
            }

            context.Logger.ChoseModelBinder(type, binder);

            return binder;
        }

        public void TryBindModel(Type type, IBindingContext context, Action<BindResult> continuation)
        {
            var binder = _binders.BinderFor(type);

            if (binder != null)
            {
                var result = executeModelBinder(type, context, () => binder.Bind(type, context));
                continuation(result);
            }
        }

        private static BindResult executeModelBinder(Type type, IBindingContext context, Func<object> source)
        {
            try
            {
                return new BindResult{
                    Value = source(),
                    Problems = context.Problems
                };
            }
            catch (Exception e)
            {
                throw new FubuException(2201, e, "Fatal error while binding model of type {0}.  See inner exception",
                                        type.AssemblyQualifiedName);
            }
        }

        public static ObjectResolver Basic()
        {
            var services = new InMemoryServiceLocator();
            var resolver = new ObjectResolver(services, new BindingRegistry(), new NulloBindingLogger());

            services.Add<IObjectResolver>(resolver);

            return resolver;
        }
    }
}