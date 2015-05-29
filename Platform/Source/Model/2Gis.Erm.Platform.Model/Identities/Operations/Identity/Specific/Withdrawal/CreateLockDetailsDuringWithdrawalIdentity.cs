using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    public class CreateLockDetailsDuringWithdrawalIdentity : OperationIdentityBase<CreateLockDetailsDuringWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CreateLockDetailsDuringWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Создание LockDetails для позиций с планируемым оказанием"; }
        }
    }
}