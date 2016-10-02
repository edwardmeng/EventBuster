using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    public interface IHandlerActionInvoker
    {
        Type EventType { get; }

        void Invoke(HandlerActionDescriptor descriptor, object evt);

#if !Net35
        Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, CancellationToken token);
#endif
    }
}
