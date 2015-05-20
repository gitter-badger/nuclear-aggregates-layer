using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IChangeDeniedPositionObjectBindingTypeOperationService : IOperation<ChangeDeniedPositionObjectBindingTypeIdentity>
    {
        void Change(long deniedPositionId, ObjectBindingType objectBindingType);
    }
}
