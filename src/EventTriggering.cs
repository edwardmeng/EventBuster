namespace EventBuster
{
    public class EventTriggering<TEvent> : CancelEvent
    {
        public EventTriggering(TEvent @event)
        {
            Event = @event;
        }

        public EventTriggering(bool cancel, TEvent @event) : base(cancel)
        {
            Event = @event;
        }

        public TEvent Event { get; }
    }

    public class EventTriggering : CancelEvent
    {
        public EventTriggering(object @event)
        {
            Event = @event;
        }

        public EventTriggering(bool cancel, object @event)
            : base(cancel)
        {
            Event = @event;
        }

        public object Event { get; }
    }
}
