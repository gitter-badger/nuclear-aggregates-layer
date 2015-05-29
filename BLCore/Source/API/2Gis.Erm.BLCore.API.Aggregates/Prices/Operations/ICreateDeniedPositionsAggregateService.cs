using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface ICreateDeniedPositionsAggregateService : IAggregateSpecificService<Price, CreateIdentity>
    {
        void Create(long priceId, long positionId, IEnumerable<DeniedPositionToCreateDto> deniedPositionDtos);
    }
}