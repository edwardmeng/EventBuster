using System;

namespace EventBuster
{
    /// <summary>
    /// The convenient static class to invoke the ambient event bus.
    /// </summary>
    public static class EventBus
    {
        #region Ambient

        private static Func<IEventBus> _eventBusFactory = () => new DefaultEventBus();
        private static IEventBus _defaultInstance;

        /// <summary>
        /// The default ambient <see cref="IEventBus"/>
        /// </summary>
        public static IEventBus Default => _defaultInstance ?? (_defaultInstance = _eventBusFactory());

        /// <summary>
        /// Set the delegate that is used to retrieve the ambient <see cref="IEventBus"/>.
        /// </summary>
        /// <param name="newEventBus">Delegate that, when called, will return the ambient <see cref="IEventBus"/>.</param>
        public static void SetEventBus(Func<IEventBus> newEventBus)
        {
            if (newEventBus == null)
            {
                throw new ArgumentNullException(nameof(newEventBus));
            }
            _eventBusFactory = newEventBus;
            _defaultInstance = null;
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        public static void Register(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            Default.Register(handler);
        }

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        public static void Register(Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            Default.Register(handlerType);
        }

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <typeparam name="THandler">The handler type.</typeparam>
        public static void Register<THandler>()
        {
            Register(typeof(THandler));
        }

#if !NetCore
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Register<TEvent>(Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(action), priority, transactionFlow);
        }
#else
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Register<TEvent>(Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(action), priority);
        }
#endif

#if NetCore

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
        }

#elif !Net35

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(HandlerActionInvoker.Create(asyncAction), priority, transactionFlow);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(HandlerActionInvoker.Create(asyncAction), priority, transactionFlow);
        }
#endif

#if !NetCore
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Register<TEvent>(Action<TEvent> action, TransactionFlowOption transactionFlow)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Register(HandlerActionInvoker.Create(action), HandlerPriority.Normal, transactionFlow);
        }
#endif
#if Net451

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(HandlerActionInvoker.Create(asyncAction), HandlerPriority.Normal, transactionFlow);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Register<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(HandlerActionInvoker.Create(asyncAction), HandlerPriority.Normal, transactionFlow);
        }

#endif

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        public static void Unregister(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            Default.Unregister(handler);
        }

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        public static void Unregister(Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            Default.Unregister(handlerType);
        }

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <typeparam name="THandler">The handler type.</typeparam>
        public static void Unregister<THandler>()
        {
            Unregister(typeof(THandler));
        }

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Unregister<TEvent>(Action<TEvent> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Unregister(HandlerActionInvoker.Create(action));
        }

#if !Net35

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Unregister<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Unregister(HandlerActionInvoker.Create(asyncAction));
        }

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static void Unregister<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Unregister(HandlerActionInvoker.Create(asyncAction));
        }

#endif

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        public static void Trigger<TEvent>(TEvent evt)
        {
            Default.Trigger(evt);
        }

#if !Net35

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <param name="token">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported. 
        /// Use 'await' to ensure that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        public static System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt, System.Threading.CancellationToken token)
        {
            return Default.TriggerAsync(evt, token);
        }

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported. 
        /// Use 'await' to ensure that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        public static System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt)
        {
            return Default.TriggerAsync(evt, System.Threading.CancellationToken.None);
        }

#endif

        #endregion
    }
}
