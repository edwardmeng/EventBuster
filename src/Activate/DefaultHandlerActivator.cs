using System;
using System.Reflection;

namespace EventBuster
{
    /// <summary>
    /// The default implementation of <see cref="IHandlerActivator"/> to instantiate the requested event handler by using reflection.
    /// </summary>
    public class DefaultHandlerActivator : IHandlerActivator
    {
        private readonly TypeActivatorCache _typeActivatorCache = new TypeActivatorCache();

        /// <summary>
        /// Creates an event handler instance.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to create event handler instance.</param>
        /// <param name="handlerType">The type of event handler.</param>
        /// <returns>The instance of requested event handler.</returns>
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

        /// <summary>
        /// Release an event handler instance
        /// </summary>
        /// <param name="handlerInstance">The instance of requested event handler.</param>
        public void Release(object handlerInstance)
        {
            (handlerInstance as IDisposable)?.Dispose();
        }
    }
}
