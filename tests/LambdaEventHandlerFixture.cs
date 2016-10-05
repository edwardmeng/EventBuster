using System;

namespace EventBuster.UnitTests
{
    public class LambdaEventHandlerFixture
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void RemoveSyncLambdaActionHandler()
        {
            object globalState = null;
            Action<CreateUserEvent> handlerAction = evt => globalState = evt.UserName;
            EventBus.Register(handlerAction);

            try
            {
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);

                EventBus.Unregister(handlerAction);
                globalState = null;
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandlerPriorityInSyncActions()
        {
            object globalState = null;
            Action<CreateRoleEvent> firstAction = evt =>
            {
                globalState = evt.RoleName + ":Step1";
                ;
            };
            Action<CreateRoleEvent> secondAction = evt =>
            {
                globalState = evt.RoleName + ":Step2";
            };
            EventBus.Register(firstAction);
            EventBus.Register(secondAction, HandlerPriority.High);

            try
            {
                EventBus.Trigger(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster:Step1", globalState);
            }
            finally
            {
                EventBus.Unregister(firstAction);
                EventBus.Unregister(secondAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleSyncEventInSyncMode()
        {
            object globalState = null;
            Action<CreateUserEvent> handlerAction = evt => globalState = evt.UserName;
            EventBus.Register(handlerAction);

            try
            {
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if !Net35

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task RemoveAsyncLambdaHandler()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction);

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);

                EventBus.Unregister(handlerAction);
                globalState = null;
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task RemoveAsyncLambdaHandlerWithToken()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction);

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" }, System.Threading.CancellationToken.None);
                Assert.Equal("EventBuster", globalState);

                EventBus.Unregister(handlerAction);
                globalState = null;
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" }, System.Threading.CancellationToken.None);
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandlerPriorityInAsyncActions()
        {
            object globalState = null;
            Func<CreateRoleEvent, System.Threading.Tasks.Task> firstAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.RoleName + ":Step1";
                return System.Threading.Tasks.Task.Delay(0);
            };
            Func<CreateRoleEvent, System.Threading.Tasks.Task> secondAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.RoleName + ":Step2";
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(firstAction);
            EventBus.Register(secondAction, HandlerPriority.High);

            try
            {
                await EventBus.TriggerAsync(new CreateRoleEvent { RoleName = "EventBuster" });
                Assert.Equal("EventBuster:Step1", globalState);
            }
            finally
            {
                EventBus.Unregister(firstAction);
                EventBus.Unregister(secondAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleSyncEventInAsyncMode()
        {
            object globalState = null;
            Action<CreateUserEvent> handlerAction = evt => globalState = evt.UserName;
            EventBus.Register(handlerAction);

            try
            {
                await EventBus.TriggerAsync(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleAsyncEventInSyncMode()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction);

            try
            {
                EventBus.Trigger(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HandleAsyncEventWithTokenInSyncMode()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction);

            try
            {
                EventBus.Trigger(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleAsyncEventInAsyncMode()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction);

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public async System.Threading.Tasks.Task HandleAsyncEventWithTokenInAsyncMode()
        {
            object globalState = null;
            Func<UpdateUserEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = evt.UserName;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction);

            try
            {
                await EventBus.TriggerAsync(new UpdateUserEvent { UserName = "EventBuster" }, System.Threading.CancellationToken.None);
                Assert.Equal("EventBuster", globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#endif

#if !NetCore

        [NUnit.Framework.Test]
        public void HandleNotAllowTransactionInSyncMode()
        {
            string globalState = null;
            Action<NotAllowTransactionEvent> handlerAction = evt =>
            {
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
            };
            EventBus.Register(handlerAction, TransactionFlowOption.NotAllowed);

            try
            {
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new NotAllowTransactionEvent());
                    transaction.Complete();
                }
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public void HandleMandatoryTransactionInScopeSyncMode()
        {
            string globalState = null;
            Action<MandatoryTransactionEvent> handlerAction = evt =>
            {
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new MandatoryTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public void HandleMandatoryTransactionOutScopeInSyncMode()
        {
            string globalState = null;
            Action<MandatoryTransactionEvent> handlerAction = evt =>
            {
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                EventBus.Trigger(new MandatoryTransactionEvent());
                Assert.NotNull(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public void HandleAllowedTransactionInScopeSyncMode()
        {
            string globalState = null;
            Action<AllowTransactionEvent> handlerAction = evt =>
            {
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    EventBus.Trigger(new AllowTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public void HandleAllowedTransactionOutScopeInSyncMode()
        {
            string globalState = null;
            Action<AllowTransactionEvent> handlerAction = evt =>
            {
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                EventBus.Trigger(new AllowTransactionEvent());
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }
#endif

#if Net451

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleNotAllowTransactionInAsyncMode()
        {
            string globalState = null;
            Func<NotAllowTransactionEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.NotAllowed);

            try
            {
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new NotAllowTransactionEvent());
                    transaction.Complete();
                }
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleNotAllowTransactionInAsyncModeWithToken()
        {
            string globalState = null;
            Func<NotAllowTransactionEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.NotAllowed);

            try
            {
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new NotAllowTransactionEvent(), System.Threading.CancellationToken.None);
                    transaction.Complete();
                }
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionInScopeAsyncMode()
        {
            string globalState = null;
            Func<MandatoryTransactionEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new MandatoryTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionInScopeAsyncModeWithToken()
        {
            string globalState = null;
            Func<MandatoryTransactionEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new MandatoryTransactionEvent(), System.Threading.CancellationToken.None);
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionOutScopeInAsyncMode()
        {
            string globalState = null;
            Func<MandatoryTransactionEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                await EventBus.TriggerAsync(new MandatoryTransactionEvent());
                Assert.NotNull(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleMandatoryTransactionOutScopeInAsyncModeWithToken()
        {
            string globalState = null;
            Func<MandatoryTransactionEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Mandatory);

            try
            {
                await EventBus.TriggerAsync(new MandatoryTransactionEvent(), System.Threading.CancellationToken.None);
                Assert.NotNull(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionInScopeAsyncMode()
        {
            string globalState = null;
            Func<AllowTransactionEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new AllowTransactionEvent());
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionInScopeAsyncModeWithToken()
        {
            string globalState = null;
            Func<AllowTransactionEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                string transactionId;
                using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    await EventBus.TriggerAsync(new AllowTransactionEvent(), System.Threading.CancellationToken.None);
                    transactionId = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                    transaction.Complete();
                }
                Assert.Equal(transactionId, globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionOutScopeInAsyncMode()
        {
            string globalState = null;
            Func<AllowTransactionEvent, System.Threading.Tasks.Task> handlerAction = evt =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                await EventBus.TriggerAsync(new AllowTransactionEvent());
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task HandleAllowedTransactionOutScopeInAsyncModeWithToken()
        {
            string globalState = null;
            Func<AllowTransactionEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> handlerAction = (evt, token) =>
            {
                System.Threading.Thread.Sleep(100);
                globalState = System.Transactions.Transaction.Current == null ? null : System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
                return System.Threading.Tasks.Task.Delay(0, token);
            };
            EventBus.Register(handlerAction, TransactionFlowOption.Allowed);

            try
            {
                await EventBus.TriggerAsync(new AllowTransactionEvent(), System.Threading.CancellationToken.None);
                Assert.Null(globalState);
            }
            finally
            {
                EventBus.Unregister(handlerAction);
            }
        }

#endif

    }
}
