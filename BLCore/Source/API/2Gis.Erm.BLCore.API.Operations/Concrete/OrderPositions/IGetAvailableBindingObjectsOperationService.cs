using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions
{
    public interface IGetAvailableBindingObjectsOperationService : IOperation<GetAvailableBinfingObjectsIdentity>
    {
        LinkingObjectsSchemaDto GetLinkingObjectsSchema(long orderId, long pricePositionId, bool includeHiddenAddresses, long? orderPositionId);
    }
}
