using System;
using System.Reflection;

namespace EventBuster
{
    public class DefaultHandlerActivator : IHandlerActivator
    {
        private readonly TypeActivatorCache _typeActivatorCache = new TypeActivatorCache();

        public object Create(IServiceProvider serviceProvider, Type handlerType)
        {
#if NetCore
            var reflectingHandlerType = handlerType.GetTypeInfo();
#else
            var reflectingHandlerType = handlerType;
#endif
            if (reflectingHandlerType.IsValueType || reflectingHandlerType.IsInterface || reflectingHandlerType.IsAbstract || (reflectingHandlerType.IsGenericType && reflectingHandlerType.IsGenericTypeDefinition))
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
