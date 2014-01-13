using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\Orders
    public interface IRepairOutdatedPositionsOperationService : IOperation<RepairOutdatedIdentity>
    {
        IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId);
        IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId, bool saveDiscounts);
    }
}
