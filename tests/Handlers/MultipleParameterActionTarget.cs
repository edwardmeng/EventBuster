namespace EventBuster.UnitTests
{
    public class MultipleParameterActionTarget
    {
        [EventHandler]
        public void HandleEvent(CreateUserEvent evt1, CreateRoleEvent evt2)
        {
            
        }
    }
}
