using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IChangeDeniedPositionObjectBindingTypeAggregateService : IAggregateSpecificOperation<Price, ChangeDeniedPositionObjectBindingTypeIdentity>
    {
        void Change(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition, ObjectBindingType newObjectBindingType);
        void ChangeSelfDenied(DeniedPosition selfDeniedPosition, ObjectBindingType newObjectBindingType);
    }
}