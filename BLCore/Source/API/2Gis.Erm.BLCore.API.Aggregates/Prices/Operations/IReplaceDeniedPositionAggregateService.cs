using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IReplaceDeniedPositionAggregateService : IAggregateSpecificService<Price, ReplaceDeniedPositionIdentity>
    {
        void Replace(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition, long positionDeniedId);
        void ReplaceSelfDenied(DeniedPosition selfDeniedPosition, long positionDeniedId);
    }
}