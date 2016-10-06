using System;

namespace EventBuster
{
    /// <summary>
    /// Provides methods to create an event handler instance.
    /// </summary>
    public interface IHandlerActivator
    {
        /// <summary>
        /// Creates an event handler instance.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to create event handler instance.</param>
        /// <param name="handlerType">The type of event handler.</param>
        /// <returns>The instance of requested event handler.</returns>
        object Create(IServiceProvider serviceProvider, Type handlerType);

        /// <summary>
        /// Release an event handler instance
        /// </summary>
        /// <param name="handlerInstance">The instance of requested event handler.</param>
        void Release(object handlerInstance);
    }
}
