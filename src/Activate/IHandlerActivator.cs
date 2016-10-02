using System;

namespace EventBuster
{
    public interface IHandlerActivator
    {
        object Create(IServiceProvider serviceProvider, Type handlerType);

        void Release(object handlerInstance);
    }
}
