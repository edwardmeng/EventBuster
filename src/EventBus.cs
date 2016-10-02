using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    public static class EventBus
    {
        #region Fields

        private static Lazy<IEventBus> _defaultInstance = new Lazy<IEventBus>(() => new DefaultEventBus());
        private static Lazy<IServiceProvider> _serviceProvider;

        #endregion

        #region Ambient

        public static ICollection<IHandlerActionDiscover> Discovers { get; } = new Collection<IHandlerActionDiscover> { new AttributeActionDiscover() };

        /// <summary>
        /// The default ambient <see cref="IEventBus"/>
        /// </summary>
        public static IEventBus Default => _defaultInstance.Value;

        /// <summary>
        /// The ambient <see cref="IServiceProvider"/>.
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = new Lazy<IServiceProvider>(() =>
                    {
                        var provider = new ServiceProvider();
                        provider.AddInstance<IHandlerActivator>(new DefaultHandlerActivator());
                        provider.Add(typeof(IEventBus), () => Default);
                        foreach (var discover in Discovers)
                        {
                            provider.AddInstance<IHandlerActionDiscover>(discover);
                        }
                        return provider;
                    });
                }
                return _serviceProvider.Value;
            }
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the ambient <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return the ambient <see cref="IServiceProvider"/>.</param>
        public static void SetServiceProvider(Func<IServiceProvider> newProvider)
        {
            if (newProvider == null)
            {
                throw new ArgumentNullException(nameof(newProvider));
            }
            _serviceProvider = new Lazy<IServiceProvider>(newProvider);
        }

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
            _defaultInstance = new Lazy<IEventBus>(newEventBus);
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
            _defaultInstance.Value.Register(handler);
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
            _defaultInstance.Value.Register(handlerType);
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
        public static void Register<TEvent>(Action<TEvent> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            _defaultInstance.Value.Register(new LambdaActionInvoker<TEvent>(action));
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Register<TEvent>(Func<TEvent, Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            _defaultInstance.Value.Register(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        /// <summary>
        /// Registers handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Register<TEvent>(Func<TEvent, CancellationToken, Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            _defaultInstance.Value.Register(new LambdaActionInvoker<TEvent>(asyncAction));
        }

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
            _defaultInstance.Value.Unregister(handler);
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
            _defaultInstance.Value.Unregister(handlerType);
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
            _defaultInstance.Value.Unregister(new LambdaActionInvoker<TEvent>(action));
        }

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Unregister<TEvent>(Func<TEvent, Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            _defaultInstance.Value.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        /// <summary>
        /// Unregisters handler action to an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The handler action.</param>
        public static void Unregister<TEvent>(Func<TEvent, CancellationToken, Task> asyncAction)
        {
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            _defaultInstance.Value.Unregister(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        public static void Trigger<TEvent>(TEvent evt)
        {
            _defaultInstance.Value.Trigger(evt);
        }

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <param name="token"></param>
        public static Task TriggerAsync<TEvent>(TEvent evt, CancellationToken token)
        {
            return _defaultInstance.Value.TriggerAsync(evt, token);
        }

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        public static Task TriggerAsync<TEvent>(TEvent evt)
        {
            return _defaultInstance.Value.TriggerAsync(evt, CancellationToken.None);
        }

        #endregion
    }
}
