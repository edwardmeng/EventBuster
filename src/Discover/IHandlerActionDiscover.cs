using System;
using System.Collections.Generic;

namespace EventBuster
{
    public interface IHandlerActionDiscover
    {
        IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, Type type);

        IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, object instance);
    }
}
