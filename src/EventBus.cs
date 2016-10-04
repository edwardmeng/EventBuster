using System;

namespace EventBuster
{
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

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
#if !NetCore
        /// <param name="transactionFlow">The transaction flow policy.</param>
#endif
        public static void Register<TEvent>(Action<TEvent> action, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
            )
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#if !NetCore
            Default.Register(new LambdaActionInvoker<TEvent>(action), priority, transactionFlow);
#else
            Default.Register(new LambdaActionInvoker<TEvent>(action), priority);
#endif
        }

#if !Net35

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
#if !NetCore
        /// <param name="transactionFlow">The transaction flow policy.</param>
#endif
        public static void Register<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
            )
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
#if !NetCore
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority, transactionFlow);
#else
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
#endif
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
#if !NetCore
        /// <param name="transactionFlow">The transaction flow policy.</param>
#endif
        public static void Register<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, HandlerPriority priority = HandlerPriority.Normal
#if !NetCore
                , TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed
#endif
            )
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
#if !NetCore
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority, transactionFlow);
#else
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), priority);
#endif
        }

#endif

#if !NetCore
        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        public static void Register<TEvent>(Action<TEvent> action, TransactionFlowOption transactionFlow)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(action), HandlerPriority.Normal, transactionFlow);
        }
#endif
#if Net451

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        public static void Register<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), HandlerPriority.Normal, transactionFlow);
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        public static void Register<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction, TransactionFlowOption transactionFlow)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Register(new LambdaActionInvoker<TEvent>(asyncAction), HandlerPriority.Normal, transactionFlow);
        }

#endif

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
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
        public static void Unregister<TEvent>(Action<TEvent> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Default.Unregister(new LambdaActionInvoker<TEvent>(action));
        }

#if !Net35
        
        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Unregister<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Unregister<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            Default.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

#endif

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
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
        /// <param name="token"></param>
        public static System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt, System.Threading.CancellationToken token)
        {
            return Default.TriggerAsync(evt, token);
        }

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        public static System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt)
        {
            return Default.TriggerAsync(evt, System.Threading.CancellationToken.None);
        }

#endif

        #endregion
    }
}
