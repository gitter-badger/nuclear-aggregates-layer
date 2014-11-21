using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Model.Metadata.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface IOrderProcessingOwnerSelectionService : IOperation<SelectOrderProcessingOwnerIdentity>
    {
        User FindOwner(OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> messages);
    }
}