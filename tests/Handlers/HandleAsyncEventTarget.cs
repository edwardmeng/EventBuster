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

        [EventHandler]
        public Task OnRoleCreatedStepOne(CreateRoleEvent evt)
        {
            Thread.Sleep(100);
            GlobalState = evt.RoleName + ":Step1";
            return Task.Delay(0);
        }

        [EventHandler(Priority = HandlerPriority.High)]
        public Task OnRoleCreateStepSecond(CreateRoleEvent evt)
        {
            Thread.Sleep(100);
            GlobalState = evt.RoleName + ":Step2";
            return Task.Delay(0);
        }

        [EventHandler(TransactionFlow = TransactionFlowOption.NotAllowed)]
        public Task HandleNotAllowedTransaction(NotAllowTransactionEvent evt)
        {
            Thread.Sleep(100);
            HandleTransaction();
            return Task.Delay(0);
        }

        [EventHandler(TransactionFlow = TransactionFlowOption.Allowed)]
        public Task HandleAllowedTransaction(AllowTransactionEvent evt)
        {
            Thread.Sleep(100);
            HandleTransaction();
            return Task.Delay(0);
        }

        [EventHandler(TransactionFlow = TransactionFlowOption.Mandatory)]
        public Task HandleMandatoryTransaction(MandatoryTransactionEvent evt)
        {
            Thread.Sleep(100);
            HandleTransaction();
            return Task.Delay(0);
        }

        private void HandleTransaction()
        {
            var transaction = System.Transactions.Transaction.Current;
            GlobalState = transaction == null ? null : transaction.TransactionInformation.LocalIdentifier;
        }
    }
}
