using NUnit.Framework;

namespace EventBuster.UnitTests
{
    public class TriggerEventFixture
    {
        [Test]
        public void HandleSyncEventInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.AreEqual("EventBuster", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [Test]
        public async System.Threading.Tasks.Task HandleSyncEventInAsyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                await EventBus.TriggerAsync(new CreateUserEvent { UserName = "EventBuster" });
                Assert.AreEqual("EventBuster", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [Test]
        public void HandlAsyncEventInSyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                EventBus.Trigger(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.AreEqual("EventBuster", HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

        [Test]
        public async System.Threading.Tasks.Task HandleAsyncEventInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.AreEqual("EventBuster", HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }
    }
}
