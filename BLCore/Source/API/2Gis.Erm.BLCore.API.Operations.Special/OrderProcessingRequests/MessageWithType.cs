using DoubleGis.Erm.BLCore.API.Operations.Metadata;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public class MessageWithType : IMessageWithType
    {
        public string MessageText { get; set; }
        public MessageType Type { get; set; }
    }
}