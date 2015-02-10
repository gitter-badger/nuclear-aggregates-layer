using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IRepairOutdatedPositionsOperationService : IOperation<RepairOutdatedIdentity>
    {
        IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId);
        IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId, bool saveDiscounts);
    }

    public sealed class RepairOutdatedPositionsOperationMessage : IMessageWithType
    {
        public string MessageText { get; set; }
        public MessageType Type { get; set; }
    }
}
