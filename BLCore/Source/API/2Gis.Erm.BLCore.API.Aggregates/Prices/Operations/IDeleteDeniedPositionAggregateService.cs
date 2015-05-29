using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IDeleteDeniedPositionAggregateService : IAggregateSpecificService<Price, DeleteIdentity>
    {
        void Delete(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition);
        void DeleteSelfDenied(DeniedPosition selfDeniedPosition);
    }
}