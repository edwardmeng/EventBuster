using System;

namespace EventBuster
{
    public class DefaultHandlerActivator : IHandlerActivator
    {
        private readonly TypeActivatorCache _typeActivatorCache = new TypeActivatorCache();

        public object Create(IServiceProvider serviceProvider, Type handlerType)
        {
            if (handlerType.IsValueType || handlerType.IsInterface || handlerType.IsAbstract || (handlerType.IsGenericType && handlerType.IsGenericTypeDefinition))
            {
                ThrowHelper.ThrowValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated(handlerType, GetType());
            }
            return _typeActivatorCache.CreateInstance(serviceProvider, handlerType);
        }

        public void Release(object handlerInstance)
        {
            (handlerInstance as IDisposable)?.Dispose();
        }
    }
}
