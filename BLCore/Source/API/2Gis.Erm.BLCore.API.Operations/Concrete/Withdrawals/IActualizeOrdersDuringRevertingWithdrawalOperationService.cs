using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IActualizeOrdersDuringRevertingWithdrawalOperationService : IOperation<ActualizeOrdersDuringRevertingWithdrawalIdentity>
    {
        void Actualize(IEnumerable<ActualizeOrdersDto> orderInfos);
    }
}