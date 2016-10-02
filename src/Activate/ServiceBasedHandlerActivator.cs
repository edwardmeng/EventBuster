using System;

namespace EventBuster
{
    public class ServiceBasedHandlerActivator : IHandlerActivator
    {
        public object Create(IServiceProvider serviceProvider, Type handlerType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            return serviceProvider.GetService(handlerType);
        }

        public void Release(object handlerInstance)
        {
        }
    }
}
