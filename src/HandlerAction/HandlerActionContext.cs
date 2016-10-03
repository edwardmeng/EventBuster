using System;
using System.Collections.Generic;

namespace EventBuster
{
    public class HandlerActionContext
    {
        private readonly IDictionary<Type, object> _instancePool;

        internal HandlerActionContext(IDictionary<Type, object> instancePool, HandlerActionDescriptor actionDescriptor, IServiceProvider serviceProvider)
        {
            ActionDescriptor = actionDescriptor;
            _instancePool = instancePool;
            Services = serviceProvider;
        }

        public HandlerActionDescriptor ActionDescriptor { get; }

        public IServiceProvider Services { get; }

        public object GetInstance(Type type)
        {
            object instance;
            if (!_instancePool.TryGetValue(type, out instance))
            {
                var activator = (IHandlerActivator)Services.GetService(typeof(IHandlerActivator));
                instance = activator.Create(Services, type);
                _instancePool.Add(type, instance);
            }
            return instance;
        }

#if !NetCore
#if Net35
        internal System.Transactions.TransactionScope CreateTransactionScope()
        {
            if (ActionDescriptor.TransactionFlow == TransactionFlowOption.NotAllowed)
            {
                if (System.Transactions.Transaction.Current != null)
                {
                    return new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Suppress);
                }
            }
            else if (ActionDescriptor.TransactionFlow == TransactionFlowOption.Mandatory)
            {
                return new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required);
            }
            return null;
        }
#else
        internal System.Transactions.TransactionScope CreateTransactionScope(System.Transactions.TransactionScopeAsyncFlowOption asyncFlowOption)
        {
            if (ActionDescriptor.TransactionFlow == TransactionFlowOption.NotAllowed)
            {
                if (System.Transactions.Transaction.Current != null)
                {
                    return new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Suppress, asyncFlowOption);
                }
            }
            else if (ActionDescriptor.TransactionFlow == TransactionFlowOption.Mandatory)
            {
                return new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, asyncFlowOption);
            }
            return null;
        }
#endif
#endif
    }
}
