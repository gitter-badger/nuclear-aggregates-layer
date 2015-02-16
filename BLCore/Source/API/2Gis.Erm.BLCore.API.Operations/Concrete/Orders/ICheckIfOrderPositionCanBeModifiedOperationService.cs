using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface ICheckIfOrderPositionCanBeModifiedOperationService : IOperation<CheckIfOrderPositionCanBeModifiedIdentity>
    {
        bool Check(long orderId, long orderPositionId, long pricePositionId, IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements, out string report);
    }
}