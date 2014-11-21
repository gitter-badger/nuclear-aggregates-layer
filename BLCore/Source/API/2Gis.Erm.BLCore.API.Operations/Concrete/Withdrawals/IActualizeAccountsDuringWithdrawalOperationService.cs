using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IActualizeAccountsDuringWithdrawalOperationService : IOperation<ActualizeAccountsDuringWithdrawalIdentity>
    {
        void Actualize(TimePeriod withdrawalPeriod, IEnumerable<AccountStateForWithdrawalDto> accountInfos);
    }
}