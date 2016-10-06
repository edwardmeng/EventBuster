using System;
using System.Collections.Generic;

namespace EventBuster
{
    /// <summary>
    /// Encapsulates information about the event handler action execution environment.
    /// </summary>
    public class HandlerActionContext
    {
        private readonly IDictionary<Type, object> _instancePool;

        internal HandlerActionContext(IDictionary<Type, object> instancePool, HandlerActionDescriptor actionDescriptor, IServiceProvider serviceProvider)
        {
            ActionDescriptor = actionDescriptor;
            _instancePool = instancePool;
            Services = serviceProvider;
        }

        /// <summary>
        /// Gets or sets the action descriptor.
        /// </summary>
        /// <value>The action descriptor.</value>
        public HandlerActionDescriptor ActionDescriptor { get; }

        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the service container. 
        /// </summary>
        public IServiceProvider Services { get; }

        internal object GetInstance(Type type)
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
