
namespace EventBuster.UnitTests
{
    public class HandleSyncEventTarget
    {
        public static object GlobalState;
        public static int CtorState;
        public string InstanceState;

        public HandleSyncEventTarget()
        {
            CtorState++;
        }

        [EventHandler]
        public void OnUserCreated(CreateUserEvent evt)
        {
            GlobalState = evt.UserName;
        }

        [EventHandler]
        public static void OnUserUpdated(UpdateUserEvent evt)
        {
            GlobalState = evt.UserName;
        }

        [EventHandler]
        public void OnRoleCreatedStepOne(CreateRoleEvent evt)
        {
            GlobalState = evt.RoleName + ":Step1";
        }

        [EventHandler(Priority = HandlerPriority.High)]
        public void OnRoleCreateStepSecond(CreateRoleEvent evt)
        {
            GlobalState = evt.RoleName + ":Step2";
        }

        [EventHandler]
        public void OnRoleUpdated(UpdateRoleEvent evt)
        {
            InstanceState = evt.RoleName;
        }

#if !NetCore
         
        [EventHandler(TransactionFlow = TransactionFlowOption.NotAllowed)]
        public void HandleNotAllowedTransaction(NotAllowTransactionEvent evt)
        {
            HandleTransaction();
        }

        [EventHandler(TransactionFlow = TransactionFlowOption.Allowed)]
        public void HandleAllowedTransaction(AllowTransactionEvent evt)
        {
            HandleTransaction();
        }

        [EventHandler(TransactionFlow = TransactionFlowOption.Mandatory)]
        public void HandleMandatoryTransaction(MandatoryTransactionEvent evt)
        {
            HandleTransaction();
        }

        private void HandleTransaction()
        {
            var transaction = System.Transactions.Transaction.Current;
            GlobalState = transaction == null ? null : transaction.TransactionInformation.LocalIdentifier;
        }

#endif
    }
}
