using NUnit.Framework;

namespace EventBuster.UnitTests
{
    public class ReflectEventHandlerFixture
    {
        [Test]
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

        [Test]
        public void HandlerPriorityInSyncActions()
        {
            EventBus.Register<HandleSyncEventTarget>();

            try
            {
                HandleSyncEventTarget.CtorState = 0;
                EventBus.Trigger(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster:Step1", HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [Test]
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

#if !Net35

        [Test]
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
         
        [Test]
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

        [Test]
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

        [Test]
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

#endif

#if !NetCore

        [Test]
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

        [Test]
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
                Assert.Equal(transactionId,HandleSyncEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<HandleSyncEventTarget>();
            }
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
