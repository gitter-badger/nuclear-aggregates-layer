using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    public class DeleteLockDetailsDuringRevertingWithdrawalIdentity : OperationIdentityBase<DeleteLockDetailsDuringRevertingWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.DeleteLockDetailsDuringRevertingWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Удаление LockDetail для позиций с планируемым оказанием"; }
        }
    }
}