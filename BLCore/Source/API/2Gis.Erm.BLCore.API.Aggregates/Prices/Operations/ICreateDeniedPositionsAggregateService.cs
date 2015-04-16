using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface ICreateDeniedPositionsAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        void Create(long priceId, long positionId, IEnumerable<DeniedPositionToCreateDto> deniedPositionDtos);
    }
}