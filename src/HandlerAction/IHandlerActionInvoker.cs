using System;

namespace EventBuster
{
    public interface IHandlerActionInvoker
    {
        Type EventType { get; }

        void Invoke(HandlerActionDescriptor descriptor, object evt);

#if !Net35
        System.Threading.Tasks.Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, System.Threading.CancellationToken token);
#endif
    }
}
