using System;

namespace EventBuster
{
    public static class EventBusExtensions
    {
        public static void Register<TEvent>(this IEventBus eventBus, Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
            )
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#if !NetCore
            eventBus.Register(new LambdaActionInvoker<TEvent>(action), priority, transactionFlow);
#else
            eventBus.Register(new LambdaActionInvoker<TEvent>(action), priority);
#endif
        }

        public static void Unregister<TEvent>(this IEventBus eventBus, Action<TEvent> action)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            eventBus.Unregister(new LambdaActionInvoker<TEvent>(action));
        }

#if !Net35
        
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
        )
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
#if !NetCore
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority, transactionFlow);
#else
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
#endif
        }

        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
        )
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
#if !NetCore
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority, transactionFlow);
#else
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
#endif
        }

        public static void Unregister<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        public static void Unregister<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

#endif

#if !NetCore

        public static void Register(this IEventBus eventBus, IHandlerActionInvoker invoker, TransactionFlowOption transactionFlow)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            eventBus.Register(invoker, HandlerPriority.Normal, transactionFlow);
        }

        public static void Register<TEvent>(this IEventBus eventBus, Action<TEvent> action, TransactionFlowOption transactionFlow)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            eventBus.Register(action, HandlerPriority.Normal, transactionFlow);
        }

#endif

#if Net451

        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(asyncAction, HandlerPriority.Normal, transactionFlow);
        }

        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(asyncAction, HandlerPriority.Normal, transactionFlow);
        }

#endif
    }
}
