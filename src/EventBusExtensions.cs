using System;

namespace EventBuster
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IEventBus"/> interface. 
    /// </summary>
    public static class EventBusExtensions
    {
#if NetCore
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="action">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(action), priority);
        }
#else
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="action">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(action), priority, transactionFlow);
        }
#endif

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="action">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
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

#if NetCore

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
        }

#elif !Net35

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(HandlerActionInvoker.Create(asyncAction), priority, transactionFlow);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(HandlerActionInvoker.Create(asyncAction), priority, transactionFlow);
        }

#endif

#if !Net35

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
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

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
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

        /// <summary>
        /// Registers handler action invoker to an event.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="invoker">The handler action invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        public static void Register(this IEventBus eventBus, IHandlerActionInvoker invoker, TransactionFlowOption transactionFlow)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            eventBus.Register(invoker, HandlerPriority.Normal, transactionFlow);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="action">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
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

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
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

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
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

#if !Net35

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="evt">Related data for the event</param>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        public static System.Threading.Tasks.Task TriggerAsync<TEvent>(this IEventBus eventBus, TEvent evt)
        {
            return eventBus.TriggerAsync(evt, System.Threading.CancellationToken.None);
        }

#endif
    }
}
