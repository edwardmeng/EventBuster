using System.Threading.Tasks;

namespace EventBuster.UnitTests
{
    public class InvalidReturnAsyncTarget
    {
        [EventHandler]
        public Task<int> HandleEvent(CreateUserEvent evt)
        {
            return Task.FromResult(0);
        }
    }
}
