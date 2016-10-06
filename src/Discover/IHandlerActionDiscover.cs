using System;
using System.Collections.Generic;

namespace EventBuster
{
    /// <summary>
    /// Providers methods to discover handler actions for specified type or instance.
    /// </summary>
    public interface IHandlerActionDiscover
    {
        /// <summary>
        /// Discovers handler actions for the specified type.
        /// </summary>
        /// <param name="type">The type to discover event handler actions.</param>
        /// <returns>The discovered event handler actions.</returns>
        IEnumerable<HandlerActionDescriptor> Discover(Type type);

        /// <summary>
        /// Discovers handler actions for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to discover event handler actions.</param>
        /// <returns>The discovered event handler actions.</returns>
        IEnumerable<HandlerActionDescriptor> Discover(object instance);
    }
}
