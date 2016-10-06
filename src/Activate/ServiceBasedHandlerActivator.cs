using System;

namespace EventBuster
{
    /// <summary>
    /// Implement <see cref="IHandlerActivator"/> to instantiate the requested event handler with <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceBasedHandlerActivator : IHandlerActivator
    {
        /// <summary>
        /// Creates an event handler instance.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to create event handler instance.</param>
        /// <param name="handlerType">The type of event handler.</param>
        /// <returns>The instance of requested event handler.</returns>
        public object Create(IServiceProvider serviceProvider, Type handlerType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            return serviceProvider.GetService(handlerType);
        }

        /// <summary>
        /// Release an event handler instance
        /// </summary>
        /// <param name="handlerInstance">The instance of requested event handler.</param>
        public void Release(object handlerInstance)
        {
        }
    }
}
