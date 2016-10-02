namespace EventBuster
{
    public class EventTriggered<TEvent>
    {
        public EventTriggered(TEvent @event)
        {
            Event = @event;
        }

        public TEvent Event { get; }
    }

    public class EventTriggered
    {
        public EventTriggered(object @event)
        {
            Event = @event;
        }

        public object Event { get; }
    }
}
