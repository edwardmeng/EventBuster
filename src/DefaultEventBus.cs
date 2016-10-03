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
                Invoker = invoker,
                Services = ServiceProvider
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
                Invoker = invoker,
                Services = ServiceProvider
            });
        }

        #endregion

        #region Trigger

        public void Trigger<TEvent>(TEvent evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));
            var isFrameworkEvent = IsFrameworkEvent(evt);
            if (!isFrameworkEvent)
            {
                var triggeringGenericEvent = new EventTriggering<TEvent>(evt);
                Trigger(triggeringGenericEvent);
                if (triggeringGenericEvent.Cancel) return;

                var triggeringEvent = new EventTriggering(evt);
                Trigger(triggeringEvent);
                if (triggeringEvent.Cancel) return;
            }
            _pool.GetPipeline(typeof(TEvent)).Invoke(evt);
            if (!isFrameworkEvent)
            {
                Trigger(new EventTriggered<TEvent>(evt));
                Trigger(new EventTriggered(evt));
            }
        }

#if !Net35
        
        public async System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt, System.Threading.CancellationToken token)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));
            var isFrameworkEvent = IsFrameworkEvent(evt);
            if (!isFrameworkEvent)
            {
                var triggeringGenericEvent = new EventTriggering<TEvent>(evt);
                await TriggerAsync(triggeringGenericEvent, token);
                if (triggeringGenericEvent.Cancel) return;

                var triggeringEvent = new EventTriggering(evt);
                await TriggerAsync(triggeringEvent, token);
                if (triggeringEvent.Cancel) return;
            }
            await _pool.GetPipeline(typeof(TEvent)).InvokeAsync(evt, token);
            if (!isFrameworkEvent)
            {
                await TriggerAsync(new EventTriggered<TEvent>(evt), token);
                await TriggerAsync(new EventTriggered(evt), token);
            }
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

        #endregion
    }
}
