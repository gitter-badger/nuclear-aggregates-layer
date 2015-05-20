using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IGetSymmetricDeniedPositionOperationService : IOperation<GetSymmetricDeniedPositionIdentity>
    {
        DeniedPosition GetSingle(long positionId, long positionDeniedId, long priceId);
        DeniedPosition GetSingleWithObjectBindingTypeConsideration(long positionId, long positionDeniedId, long priceId, ObjectBindingType objectBindingType);
        DeniedPosition GetFirstInactiveWithObjectBindingTypeConsideration(long positionId, long positionDeniedId, long priceId, ObjectBindingType objectBindingType);
    }
}
