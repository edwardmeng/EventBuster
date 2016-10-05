namespace EventBuster.UnitTests
{
    public class LegalEventTarget
    {
        [EventHandler]
        public void HandleSyncEvent(CreateUserEvent evt)
        {
        }

        [EventHandler]
        public static void HandleStaticSyncEvent(CreateUserEvent evt)
        {
        }

#if !Net35
         
        [EventHandler]
        public System.Threading.Tasks.Task HandleAsyncEvent(CreateUserEvent evt)
        {
            return System.Threading.Tasks.Task.Delay(0);
        }

        [EventHandler]
        public static System.Threading.Tasks.Task HandleStaticAsyncEvent(CreateUserEvent evt)
        {
            return System.Threading.Tasks.Task.Delay(0);
        }

        [EventHandler]
        public System.Threading.Tasks.Task HandleAsyncEventWithToken(CreateUserEvent evt, System.Threading.CancellationToken token)
        {
            return System.Threading.Tasks.Task.Delay(0, token);
        }

        [EventHandler]
        public static System.Threading.Tasks.Task HandleStaticAsyncEventWithToken(CreateUserEvent evt, System.Threading.CancellationToken token)
        {
            return System.Threading.Tasks.Task.Delay(0, token);
        }

#endif
    }
}
