using System;
using System.Collections.Generic;

namespace EventBuster
{
    /// <summary>
    /// The generic event bus interface. This interface is used to register/unregister event handlers and trigger events.
    /// </summary>
    public interface IEventBus
    {
        #region Ambient

        /// <summary>
        /// Gets the collection of <see cref="IHandlerActionDiscover"/> to detect event handler action descriptors.
        /// </summary>
        ICollection<IHandlerActionDiscover> Discovers { get; }

        /// <summary>
        /// Set the delegate that is used to retrieve the ambient <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return the ambient <see cref="IServiceProvider"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="newProvider"/> is null.</exception>
        void SetServiceProvider(Func<IServiceProvider> newProvider);

        #endregion

        #region Register

        /// <summary>
        /// Registers handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        void Register(object handler);

        /// <summary>
        /// Registers handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        void Register(Type handlerType);

#if NetCore
        /// <summary>
        /// Registers handler action invoker to an event.
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        void Register(IHandlerActionInvoker invoker, HandlerPriority priority = HandlerPriority.Normal);
#else
        /// <summary>
        /// Registers handler action invoker to an event.
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <param name="priority">The execute priority of the invoker.</param>
        /// <param name="transactionFlow">The transaction flow policy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        void Register(IHandlerActionInvoker invoker, HandlerPriority priority = HandlerPriority.Normal, TransactionFlowOption transactionFlow = TransactionFlowOption.Allowed);
#endif

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters handler to an event. 
        /// </summary>
        /// <param name="handler">The event handle instance to handle event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        void Unregister(object handler);

        /// <summary>
        /// Unregisters handler type to an event.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is null.</exception>
        void Unregister(Type handlerType);

        /// <summary>
        /// Unregisters handler action invoker to an event.
        /// </summary>
        /// <param name="invoker">The handler action invoker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invoker"/> is null.</exception>
        void Unregister(IHandlerActionInvoker invoker);

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        void Trigger<TEvent>(TEvent evt);

#if !Net35

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="evt">Related data for the event</param>
        /// <param name="token">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the mapped target object.</returns>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported. 
        /// Use 'await' to ensure that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is null.</exception>
        System.Threading.Tasks.Task TriggerAsync<TEvent>(TEvent evt, System.Threading.CancellationToken token);

#endif

        #endregion
    }
}
