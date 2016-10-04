namespace EventBuster.UnitTests
{
    public class CustomEventTarget
    {
        public static object GlobalState;

        public void CustomAction(UpdateUserEvent evt)
        {
            GlobalState = evt.UserName;
        }
    }
}
