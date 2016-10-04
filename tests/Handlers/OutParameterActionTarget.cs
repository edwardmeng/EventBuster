namespace EventBuster.UnitTests
{
    public class OutParameterActionTarget
    {
        [EventHandler]
        public void HandleEvent(out CreateUserEvent evt)
        {
            evt = null;
        }
    }
}
