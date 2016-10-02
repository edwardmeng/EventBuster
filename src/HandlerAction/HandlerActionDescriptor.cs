using System;
using System.Transactions;

namespace EventBuster
{
    public class HandlerActionDescriptor
    {
        public HandlerPriority Priority { get; set; } = HandlerPriority.Normal;

        public IHandlerActionInvoker Invoker { get; set; }

        public IServiceProvider Services { get; set; }

        public TransactionFlowOption TransactionFlow { get; set; } = TransactionFlowOption.Allowed;

        public Type EventType => Invoker?.EventType;

        internal TransactionScope CreateTransactionScope(TransactionScopeAsyncFlowOption asyncFlowOption)
        {
            if (TransactionFlow == TransactionFlowOption.NotAllowed)
            {
                if (Transaction.Current != null)
                {
                    return new TransactionScope(TransactionScopeOption.Suppress, asyncFlowOption);
                }
            }
            else if (TransactionFlow == TransactionFlowOption.Mandatory)
            {
                return new TransactionScope(TransactionScopeOption.Required, asyncFlowOption);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as HandlerActionDescriptor;
            if (other == null) return false;
            if (obj.GetType() != GetType()) return false;
            return Priority == other.Priority && Equals(Invoker, other.Invoker) && TransactionFlow == other.TransactionFlow;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Priority;
                hashCode = (hashCode * 397) ^ (Invoker?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int)TransactionFlow;
                return hashCode;
            }
        }
    }
}
