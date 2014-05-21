using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations
{
    public interface ICreateChargesHistoryAggregateService : IAggregateSpecificOperation<WithdrawalInfo, CreateIdentity>
    {
        void Create(ChargesHistoryDto item);
    }
}