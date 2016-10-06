namespace EventBuster
{
    /// <summary>
    /// The framework event that triggered 
    /// while any application event is triggering
    /// and the event type is same as <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of triggering event.</typeparam>
    public class EventTriggering<TEvent> : CancelEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggering{TEvent}"/> class with the <see cref="Event"/> property set to <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The data of the triggering event.</param>
        public EventTriggering(TEvent @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggering{TEvent}"/> class 
        /// with the <see cref="Event"/> property set to <paramref name="event"/>
        /// and the <see cref="CancelEvent.Cancel"/> property set to false.
        /// </summary>
        /// <param name="cancel"><c>true</c> to cancel the event; otherwise, <c>false</c>.</param>
        /// <param name="event">The data of the triggering event.</param>
        public EventTriggering(bool cancel, TEvent @event) : base(cancel)
        {
            Event = @event;
        }

        /// <summary>
        /// Gets the data of the triggering event.
        /// </summary>
        public TEvent Event { get; }
    }

    /// <summary>
    /// The framework event that triggered 
    /// while any application event is triggering.
    /// </summary>
    public class EventTriggering : CancelEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggering"/> class with the <see cref="Event"/> property set to <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The data of the triggering event.</param>
        public EventTriggering(object @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggering"/> class 
        /// with the <see cref="Event"/> property set to <paramref name="event"/>
        /// and the <see cref="CancelEvent.Cancel"/> property set to false.
        /// </summary>
        /// <param name="event">The data of the triggering event.</param>
        /// <param name="cancel"><c>true</c> to cancel the event; otherwise, <c>false</c>.</param>
        public EventTriggering(bool cancel, object @event)
            : base(cancel)
        {
            Event = @event;
        }

        /// <summary>
        /// Gets the data of the triggering event.
        /// </summary>
        public object Event { get; }
    }
}
