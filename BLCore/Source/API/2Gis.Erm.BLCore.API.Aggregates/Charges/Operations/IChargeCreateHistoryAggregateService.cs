using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations
{
    public interface IChargeCreateHistoryAggregateService : IAggregateSpecificOperation<Charge, CreateIdentity>
    {
        void Create(ChargesHistoryDto item);
    }
}