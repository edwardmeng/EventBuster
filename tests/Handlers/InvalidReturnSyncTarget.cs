namespace EventBuster.UnitTests
{
    public class InvalidReturnSyncTarget
    {
        [EventHandler]
        public int HandleEvent(CreateUserEvent evt)
        {
            return 0;
        }
    }
}
