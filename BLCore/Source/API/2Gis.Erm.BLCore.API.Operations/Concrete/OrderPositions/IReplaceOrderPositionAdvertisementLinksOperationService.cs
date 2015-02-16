using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions
{
    public interface IReplaceOrderPositionAdvertisementLinksOperationService : IOperation<ReplaceOrderPositionAdvertisementLinksIdentity>
    {
        void Replace(long orderPositionId, IReadOnlyList<AdvertisementDescriptor> advertisementLinkDescriptors);
    }
}
