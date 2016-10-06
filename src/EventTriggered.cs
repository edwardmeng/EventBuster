namespace EventBuster
{
    /// <summary>
    /// The framework event that triggered 
    /// while any application event is triggered
    /// and the event type is same as <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of triggered event.</typeparam>
    public class EventTriggered<TEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggered{TEvent}"/> class with the <see cref="Event"/> property set to <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The data of the triggered event.</param>
        public EventTriggered(TEvent @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Gets the data of the triggered event.
        /// </summary>
        public TEvent Event { get; }
    }

    /// <summary>
    /// The framework event that triggered 
    /// while any application event is triggered.
    /// </summary>
    public class EventTriggered
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTriggered"/> class with the <see cref="Event"/> property set to <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The data of the triggered event.</param>
        public EventTriggered(object @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Gets the data of the triggered event.
        /// </summary>
        public object Event { get; }
    }
}
