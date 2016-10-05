using System;

namespace EventBuster.UnitTests
{
    public class ReflectEventHandlerFixture
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ActivateOnceWhenMultipleAction()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.CtorState = 0;
                EventBus.Trigger(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal(1, HandleSyncEventTarget.CtorState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandlerPriorityInSyncActions()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                EventBus.Trigger(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster:Step1", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleSyncEventInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleInstanceEventInSyncMode()
        {
            var instance = new HandleSyncEventTarget();
            EventBus.Register(instance);
            try
            {
                EventBus.Trigger(new UpdateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster", instance.InstanceState);
            }
            finally
            {
                EventBus.Unregister(instance);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ThrowExceptionInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();
            try
            {
                Assert.Throws<NotSupportedException>(() => EventBus.Trigger(new CancelEvent()));
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#if !Net35

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandlerPriorityInAsyncActions()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleSyncEventTarget.CtorState = 0;
                await EventBus.TriggerAsync(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster:Step1", HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleSyncEventInAsyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                await EventBus.TriggerAsync(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleAsyncEventInSyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                EventBus.Trigger(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleAsyncEventInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleInstanceEventInAsyncMode()
        {
            var instance = new HandleAsyncEventTarget();
            EventBus.Register(instance);
            try
            {
                await EventBus.TriggerAsync(new UpdateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster", instance.InstanceState);
            }
            finally
            {
                EventBus.Unregister(instance);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task ThrowAsyncExceptionInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();
            try
            {
                await Assert.ThrowsAsync<NotSupportedException>(() => EventBus.TriggerAsync(new CancelEvent()));
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ThrowAsyncExceptionInSyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();
            try
            {
                Assert.Throws<NotSupportedException>(() => EventBus.Trigger(new CancelEvent()));
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#endif

#if !NetCore

        [NUnit.Framework.Test]
        public void HandleNotAllowTransactionInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.GlobalState = null;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new NotAllowTransactionEvent());
                    transaction.Complete();
                }
                Assert.Null(HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public void HandleMandatoryTransactionInScopeSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.GlobalState = null;
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new MandatoryTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public void HandleMandatoryTransactionOutScopeInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.GlobalState = null;
                EventBus.Trigger(new MandatoryTransactionEvent());
                Assert.NotNull(HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public void HandleAllowedTransactionInScopeSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.GlobalState = null;
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new AllowTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public void HandleAllowedTransactionOutScopeInSyncMode()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.GlobalState = null;
                EventBus.Trigger(new AllowTransactionEvent());
                Assert.Null(HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

#endif

#if Net451

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleNotAllowTransactionInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleAsyncEventTarget.GlobalState = null;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new NotAllowTransactionEvent());
                    transaction.Complete();
                }
                Assert.Null(HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionInScopeAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleAsyncEventTarget.GlobalState = null;
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new MandatoryTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionOutScopeInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleAsyncEventTarget.GlobalState = null;
                await EventBus.TriggerAsync(new MandatoryTransactionEvent());
                Assert.NotNull(HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionInScopeAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleAsyncEventTarget.GlobalState = null;
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new AllowTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionOutScopeInAsyncMode()
        {
            EventBus.Register<HandleAsyncEventTarget>();

            try
            {
                HandleAsyncEventTarget.GlobalState = null;
                await EventBus.TriggerAsync(new AllowTransactionEvent());
                Assert.Null(HandleAsyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleAsyncEventTarget>();
            }
        }

#endif
    }
}
