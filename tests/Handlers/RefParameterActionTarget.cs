namespace EventBuster.UnitTests
{
    public class RefParameterActionTarget
    {
        [EventHandler]
        public void HandleEvent(ref CreateUserEvent evt)
        {
            
        }
    }
}
