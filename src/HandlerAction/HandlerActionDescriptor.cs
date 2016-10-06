using System;

namespace EventBuster
{
    /// <summary>
    /// Provides information about an action method, such as its priority, event type, invoker and transaction flow option.
    /// </summary>
    public class HandlerActionDescriptor
    {
        /// <summary>
        /// Gets or sets the invocation priority of the target action method.
        /// </summary>
        public HandlerPriority Priority { get; set; } = HandlerPriority.Normal;

        /// <summary>
        /// Gets or sets the invoker of the target action method.
        /// </summary>
        public IHandlerActionInvoker Invoker { get; set; }

        /// <summary>
        /// Gets the event type of target action method.
        /// </summary>
        public Type EventType => Invoker?.EventType;

#if !NetCore

        /// <summary>
        /// Gets or sets the transaction flow option of target action method.
        /// </summary>
        public TransactionFlowOption TransactionFlow { get; set; } = TransactionFlowOption.Allowed;
#endif

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an instance of <see cref="HandlerActionDescriptor"/> and equals the value of this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as HandlerActionDescriptor;
            if (other == null) return false;
            if (obj.GetType() != GetType()) return false;
#if !NetCore
            if (TransactionFlow != other.TransactionFlow) return false;
#endif
            return Priority == other.Priority && Equals(Invoker, other.Invoker);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Priority;
                hashCode = (hashCode * 397) ^ (Invoker?.GetHashCode() ?? 0);
#if !NetCore
                hashCode = (hashCode * 397) ^ (int)TransactionFlow;
#endif
                return hashCode;
            }
        }
    }
}
