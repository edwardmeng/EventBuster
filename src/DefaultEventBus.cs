using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    /// <summary>
    /// The default implementation of <see cref="IEventBus"/>.
    /// </summary>
    internal class DefaultEventBus : IEventBus
    {
        #region Fields

        private readonly HandlerActionPool _pool = new HandlerActionPool();
        private Func<IServiceProvider> _serviceProviderFactory;
        private IServiceProvider _serviceProvider;

        #endregion

        #region Ambient

        /// <summary>
        /// Gets the collection of <see cref="IHandlerActionDiscover"/> to detect event handler action descriptors.
        /// </summary>
        public ICollection<IHandlerActionDiscover> Discovers { get; } = new Collection<IHandlerActionDiscover> { new AttributeActionDiscover() };

        /// <summary>
        /// The ambient <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    if (_serviceProviderFactory == null)
                    {
                        _serviceProviderFactory = () =>
                        {
                            var serviceProvider = new ServiceProvider();
                            serviceProvider.AddInstance<IHandlerActivator>(new DefaultHandlerActivator());
                            serviceProvider.Add(typeof(IEventBus), () => this);
                            foreach (var discover in Discovers)
                            {
                                serviceProvider.AddInstance<IHandlerActionDiscover>(discover);
                            }
                            return serviceProvider;
                        };
                    }
                    _serviceProvider = _serviceProviderFactory();
                }
                return _serviceProvider;
            }
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the ambient <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return the ambient <see cref="IServiceProvider"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="newProvider"/> is null.</exception>
        public void SetServiceProvider(Func<IServiceProvider> newProvider)
        {
            if (newProvider == null)
            {
                throw new ArgumentNullException(nameof(newProvider));
            }
            _serviceProviderFactory = newProvider;
            _serviceProvider = null;
        }

        internal void ResetServiceProvider()
        {
            _serviceProviderFactory = null;
            _serviceProvider = null;
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        public void Register(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(handler)))
            {
                _pool.Add(actionDescriptor);
            }
        }

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        public void Register(Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(handlerType)))
            {
                _pool.Add(actionDescriptor);
            }
        }
#if NetCore
        /// <summary>
        /// Registers handler action invoker to an event. 
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        public void Register(IHandlerActionInvoker invoker, HandlerPriority priority = HandlerPriority.Normal)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException(nameof(invoker));
            }
            _pool.Add(new HandlerActionDescriptor
            {
                Invoker = invoker,
                Priority = priority
            });
        }
#else
        /// <summary>
        /// Registers handler action invoker to an event. 
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        public void Register(IHandlerActionInvoker invoker, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException(nameof(invoker));
            }
            _pool.Add(new HandlerActionDescriptor
            {
                Invoker = invoker,
                Priority = priority,
                TransactionFlow = transactionFlow
            });
        }
#endif

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        public void Unregister(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(handler)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        public void Unregister(Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(handlerType)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        /// <summary>
        /// Unregisters handler action invoker to an event. 
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        public void Unregister(IHandlerActionInvoker invoker)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException(nameof(invoker));
            }
            _pool.Remove(invoker);
        }

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        public void Trigger<TEvent>(TEvent evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));
            var instancePool = new Dictionary<Type, object>();
            try
            {
                var isFrameworkEvent = IsFrameworkEvent(evt);
                if (!isFrameworkEvent)
                {
                    var triggeringGenericEvent = new EventTriggering<TEvent>(evt);
                    TriggerInternal(instancePool, triggeringGenericEvent);
                    if (triggeringGenericEvent.Cancel) return;

                    var triggeringEvent = new EventTriggering(evt);
                    TriggerInternal(instancePool, triggeringEvent);
                    if (triggeringEvent.Cancel) return;
                }
                TriggerInternal(instancePool, evt);
                if (!isFrameworkEvent)
                {
                    TriggerInternal(instancePool, new EventTriggered<TEvent>(evt));
                    TriggerInternal(instancePool, new EventTriggered(evt));
                }
            }
            finally
            {
                ReleaseInstances(instancePool);
            }
        }

        private void TriggerInternal<TEvent>(IDictionary<Type, object> instancePool, TEvent evt)
        {
            _pool.GetPipeline(typeof(TEvent)).Invoke(ServiceProvider, instancePool, evt);
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
        public async System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt, System.Threading.CancellationToken token)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));
            var instancePool = new Dictionary<Type, object>();
            try
            {
                var isFrameworkEvent = IsFrameworkEvent(evt);
                if (!isFrameworkEvent)
                {
                    var triggeringGenericEvent = new EventTriggering<TEvent>(evt);
                    await TriggerAsyncInternal(instancePool, triggeringGenericEvent, token);
                    if (triggeringGenericEvent.Cancel) return;

                    var triggeringEvent = new EventTriggering(evt);
                    await TriggerAsyncInternal(instancePool, triggeringEvent, token);
                    if (triggeringEvent.Cancel) return;
                }
                await TriggerAsyncInternal(instancePool, evt, token);
                if (!isFrameworkEvent)
                {
                    await TriggerAsyncInternal(instancePool, new EventTriggered<TEvent>(evt), token);
                    await TriggerAsyncInternal(instancePool, new EventTriggered(evt), token);
                }
            }
            finally
            {
                ReleaseInstances(instancePool);
            }
        }

        private async System.Threading.Tasks.Task TriggerAsyncInternal<TEvent>(IDictionary<Type, object> instancePool, TEvent evt, System.Threading.CancellationToken token)
        {
            await _pool.GetPipeline(typeof(TEvent)).InvokeAsync(ServiceProvider, instancePool, evt, token);
        }

#endif

        private bool IsFrameworkEvent(object evt)
        {
#if NetCore
            return Equals(evt.GetType().GetTypeInfo().Assembly, GetType().GetTypeInfo().Assembly);
#else
            return Equals(evt.GetType().Assembly, GetType().Assembly);
#endif
        }

        private void ReleaseInstances(IDictionary<Type, object> instancePool)
        {
            var activator = (IHandlerActivator)ServiceProvider.GetService(typeof(IHandlerActivator));
            foreach (var instance in instancePool.Values)
            {
                if (instance != null)
                {
                    activator.Release(instance);
                }
            }
        }

#endregion
    }
}
