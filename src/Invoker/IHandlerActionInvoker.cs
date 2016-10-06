using System;

namespace EventBuster
{
    /// <summary>
    /// Defines the contract for an event handler action invoker.
    /// </summary>
    public interface IHandlerActionInvoker
    {
        /// <summary>
        /// Gets the type of target event argument.
        /// </summary>
        Type EventType { get; }

        /// <summary>
        /// Invokes the specified action by using the specified context and event argument.
        /// </summary>
        /// <param name="context">The event handler action context.</param>
        /// <param name="evt">The event argument.</param>
        void Invoke(HandlerActionContext context, object evt);

#if !Net35

        /// <summary>
        /// Gets an value to indicate the target action should be invoked asynchronously.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Invokes the specified action asynchronously by using the specified context and event argument.
        /// </summary>
        /// <param name="context">The event handler action context.</param>
        /// <param name="evt">The event argument.</param>
        /// <param name="token">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported. 
        /// Use 'await' to ensure that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        System.Threading.Tasks.Task InvokeAsync(HandlerActionContext context, object evt, System.Threading.CancellationToken token);
#endif
    }
}
