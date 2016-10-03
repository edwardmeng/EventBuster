using System;

namespace EventBuster
{
    public interface IHandlerActionInvoker
    {
        Type EventType { get; }

        void Invoke(HandlerActionContext context, object evt);

#if !Net35
        System.Threading.Tasks.Task InvokeAsync(HandlerActionContext context, object evt, System.Threading.CancellationToken token);
#endif
    }
}
