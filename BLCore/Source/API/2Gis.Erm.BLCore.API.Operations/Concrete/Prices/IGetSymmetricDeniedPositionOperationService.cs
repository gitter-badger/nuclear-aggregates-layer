using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IGetSymmetricDeniedPositionOperationService : IOperation<GetSymmetricDeniedPositionIdentity>
    {
        DeniedPosition Get(DeniedPosition deniedPosition);
        DeniedPosition GetWithObjectBindingTypeConsideration(DeniedPosition deniedPosition);
        DeniedPosition GetInactiveWithObjectBindingTypeConsideration(DeniedPosition deniedPosition);
    }
}
