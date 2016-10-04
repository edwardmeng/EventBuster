namespace EventBuster.UnitTests
{
    public class HandleSyncEventTarget
    {
        public static object GlobalState;
        [EventHandler]
        public void OnUserCreated(CreateUserEvent evt)
        {
            GlobalState = evt.UserName;
        }

        [EventHandler]
        public static void OnUserUpdated(UpdateUserEvent evt)
        {
            GlobalState = evt.UserName;
        }
    }
}
