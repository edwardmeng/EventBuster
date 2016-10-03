using System;

namespace EventBuster
{
    public class HandlerActionDescriptor
    {
        public HandlerPriority Priority { get; set; } = HandlerPriority.Normal;

        public IHandlerActionInvoker Invoker { get; set; }

        public Type EventType => Invoker?.EventType;

#if !NetCore
        public TransactionFlowOption TransactionFlow { get; set; } = TransactionFlowOption.Allowed;
#endif

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
