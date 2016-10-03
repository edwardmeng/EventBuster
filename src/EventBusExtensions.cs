using System;

namespace EventBuster
{
    public static class EventBusExtensions
    {
        public static void Register<TEvent>(this IEventBus eventBus, Action<TEvent> action)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(action));
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
        
        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.Tasks.Task> asyncAction)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction));
        }

        public static void Register<TEvent>(this IEventBus eventBus, Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            if (asyncAction == null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }
            eventBus.Register(new LambdaActionInvoker<TEvent>(asyncAction));
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
    }
}
