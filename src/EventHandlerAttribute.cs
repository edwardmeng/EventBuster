using System;

namespace EventBuster
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating the invocation priority of an event handler method.
        /// </summary>
        /// <value>One of the <see cref="HandlerPriority"/> values. The default value is <see cref="HandlerPriority.Normal"/>.</value>
        public HandlerPriority Priority { get; set; } = HandlerPriority.Normal;

#if !NetCore
        
        /// <summary>
        /// Gets a value that indicates whether the incoming transaction is supported.
        /// </summary>
        /// <value>A <see cref="TransactionFlowOption"/> that indicates whether the incoming transaction is supported.</value>
        public TransactionFlowOption TransactionFlow { get; set; } = TransactionFlowOption.Allowed;
#endif
    }
}
