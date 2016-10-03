using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    internal class DefaultEventBus : IEventBus
    {
        private readonly HandlerActionPool _pool = new HandlerActionPool();
        private Func<IServiceProvider> _serviceProviderFactory;
        private IServiceProvider _serviceProvider;

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
        public void SetServiceProvider(Func<IServiceProvider> newProvider)
        {
            if (newProvider == null)
            {
                throw new ArgumentNullException(nameof(newProvider));
            }
            _serviceProviderFactory = newProvider;
            _serviceProvider = null;
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        public void Register(object handler)
        {
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(ServiceProvider, handler)))
            {
                _pool.Add(actionDescriptor);
            }
        }

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        public void Register(Type handlerType)
        {
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(ServiceProvider, handlerType)))
            {
                _pool.Add(actionDescriptor);
            }
        }

        /// <summary>
        /// Registers handler action invoker to an event. 
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        public void Register(IHandlerActionInvoker invoker)
        {
            _pool.Add(new HandlerActionDescriptor
            {
                Invoker = invoker
            });
        }

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        public void Unregister(object handler)
        {
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(ServiceProvider, handler)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        public void Unregister(Type handlerType)
        {
            foreach (var actionDescriptor in Discovers.SelectMany(discover => discover.Discover(ServiceProvider, handlerType)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        /// <summary>
        /// Unregisters handler action invoker to an event. 
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        public void Unregister(IHandlerActionInvoker invoker)
        {
            _pool.Remove(new HandlerActionDescriptor
            {
                Invoker = invoker
            });
        }

        #endregion

        #region Trigger

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
