using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IWithdrawalOperationService : IOperation<WithdrawalIdentity>
    {
        void Withdraw(long organizationUnitId, TimePeriod period);
    }
}
