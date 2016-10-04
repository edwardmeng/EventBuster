using System.Threading;
using System.Threading.Tasks;

namespace EventBuster.UnitTests
{
    public class HandleAsyncEventTarget
    {
        public static object GlobalState;
        [EventHandler]
        public Task OnUserUpdated(UpdateUserEvent evt)
        {
            Thread.Sleep(100);
            GlobalState = evt.UserName;
            return Task.Delay(0);
        }

        [EventHandler]
        public static Task OnUserCreated(CreateUserEvent evt)
        {
            Thread.Sleep(100);
            GlobalState = evt.UserName;
            return Task.Delay(0);
        }
    }
}
