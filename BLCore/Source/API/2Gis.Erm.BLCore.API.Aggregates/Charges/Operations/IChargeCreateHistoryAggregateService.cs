using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations
{
    public interface IChargeCreateHistoryAggregateService : IAggregateSpecificService<Charge, CreateIdentity>
    {
        void Create(ChargesHistoryDto item);
    }
}