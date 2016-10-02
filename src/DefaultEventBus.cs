using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    internal class DefaultEventBus : IEventBus
    {
        private readonly HandlerActionPool _pool = new HandlerActionPool();

        private bool IsFrameworkEvent(object evt)
        {
            return evt.GetType().Assembly == GetType().Assembly;
        }

        public void Register(object handler)
        {
            foreach (var actionDescriptor in EventBus.Discovers.SelectMany(discover => discover.Discover(EventBus.ServiceProvider, handler)))
            {
                _pool.Add(actionDescriptor);
            }
        }

        public void Register(Type handlerType)
        {
            foreach (var actionDescriptor in EventBus.Discovers.SelectMany(discover => discover.Discover(EventBus.ServiceProvider, handlerType)))
            {
                _pool.Add(actionDescriptor);
            }
        }

        public void Register(IHandlerActionInvoker invoker)
        {
            _pool.Add(new HandlerActionDescriptor
            {
                Invoker = invoker,
                Services = EventBus.ServiceProvider
            });
        }

        public void Unregister(object handler)
        {
            foreach (var actionDescriptor in EventBus.Discovers.SelectMany(discover => discover.Discover(EventBus.ServiceProvider, handler)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        public void Unregister(Type handlerType)
        {
            foreach (var actionDescriptor in EventBus.Discovers.SelectMany(discover => discover.Discover(EventBus.ServiceProvider, handlerType)))
            {
                _pool.Remove(actionDescriptor);
            }
        }

        public void Unregister(IHandlerActionInvoker invoker)
        {
            _pool.Remove(new HandlerActionDescriptor
            {
                Invoker = invoker,
                Services = EventBus.ServiceProvider
            });
        }

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

        public async Task TriggerAsync<TEvent>(TEvent evt, CancellationToken token)
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
    }
}
