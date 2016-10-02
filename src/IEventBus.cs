using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    public interface IEventBus
    {
        #region Register

        /// <summary>
        /// Registers handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        void Register(object handler);

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        void Register(Type handlerType);

        /// <summary>
        /// Registers handler action invoker to an event.
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        void Register(IHandlerActionInvoker invoker);

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        void Unregister(object handler);

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        void Unregister(Type handlerType);

        /// <summary>
        /// Unregisters handler action invoker to an event.
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        void Unregister(IHandlerActionInvoker invoker);

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        void Trigger<TEvent>(TEvent evt);

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <param name="token"></param>
        Task TriggerAsync<TEvent>(TEvent evt, CancellationToken token);

        #endregion
    }
}
