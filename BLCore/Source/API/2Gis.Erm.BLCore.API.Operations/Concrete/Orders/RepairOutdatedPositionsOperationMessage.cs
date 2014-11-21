using DoubleGis.Erm.BLCore.API.Operations.Metadata;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class RepairOutdatedPositionsOperationMessage : IMessageWithType
    {
        public string MessageText { get; set; }
        public MessageType Type { get; set; }
    }
}
