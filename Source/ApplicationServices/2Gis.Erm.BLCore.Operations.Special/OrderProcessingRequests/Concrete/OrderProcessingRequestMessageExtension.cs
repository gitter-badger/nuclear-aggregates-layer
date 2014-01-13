using DoubleGis.Erm.BLCore.Aggregates.CommonService;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Messages;

using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    public static class OrderProcessingRequestMessageExtension
    {
        public static OrderProcessingRequestMessage ToOrderProcessingRequestMessage(this IMessageWithType message, long orderProcessingRequestId)
        {
            return new OrderProcessingRequestMessage
                {
                    MessageTemplateCode = MessageCodes.GeneralMessage,
                    MessageParameters = MessageHelper.PrepareParametersContext(message.MessageText),
                    MessageType = (int)GetOrderProcessingRequestMessageType(message.Type),
                    OrderRequestId = orderProcessingRequestId,
                    IsActive = true
                };
        }

        private static RequestMessageType GetOrderProcessingRequestMessageType(MessageType source)
        {
            switch (source)
            {
                case MessageType.Info:
                    return RequestMessageType.Info;
                case MessageType.Warning:
                    return RequestMessageType.Warning;
                case MessageType.Error:
                    return RequestMessageType.Error;
                case MessageType.Debug:
                    return RequestMessageType.Debug;
                default:
                    return RequestMessageType.None;
            }
        }
    }
}
