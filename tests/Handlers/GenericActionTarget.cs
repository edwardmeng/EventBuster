namespace EventBuster.UnitTests
{
    public class GenericActionTarget
    {
        [EventHandler]
        public void HandleEvent<TEvent>(TEvent evt)
        {
            
        }
    }
}
