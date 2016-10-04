namespace EventBuster.UnitTests
{
    public class CustomActivateTarget
    {
        public static string StaticState;
        public string InstanceState;

        [EventHandler]
        public void OnUserCreated(CreateUserEvent evt)
        {
            InstanceState = evt.UserName;
        }

        public string CustomState
        {
            get { return StaticState; }
            set { StaticState = value; }
        }
    }
}
