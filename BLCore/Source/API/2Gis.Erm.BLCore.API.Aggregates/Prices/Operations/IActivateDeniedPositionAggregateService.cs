using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IActivateDeniedPositionAggregateService : IAggregateSpecificOperation<Price, ActivateIdentity>
    {
        void Activate(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition);
        void ActivateSelfDenied(DeniedPosition selfDeniedPosition);
    }
}