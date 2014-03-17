using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Services.Notifications;
using DoubleGis.Erm.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // TODO {all, 17.12.2013}: Лучше разделить форматирование текста для операций создания заявки, реализации заявки и, возможно, других операций по заявке. 
    public class OrderProcessingRequestNotificationFormatter : IOrderProcessingRequestNotificationFormatter
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private static readonly MessageType[] AllowedMessageTypes = { MessageType.Error, MessageType.Warning };

        public OrderProcessingRequestNotificationFormatter(IOrderProcessingRequestService orderProcessingRequestService)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
        }

        public NotificationMessage Format(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, IEnumerable<IMessageWithType> messagesToSend)
        {
            var result = new NotificationMessage
                {
                    Subject = GetMessageSubject(orderProcessingRequest),
                    Body = GetMessageBody(orderProcessingRequest, messagesToSend)
                };
                                               
            return result;
        }

        private static string GetMessageSubject(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            return orderProcessingRequest.RequestType == (int)OrderProcessingRequestType.CreateOrder
                       ? BLResources.OrderProcessingRequestCreationResultEmailSubject
                       : BLResources.OrderProcessingRequestProlongationResultEmailSubject;
        }

        private string GetMessageBody(
            Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest,
            IEnumerable<IMessageWithType> messagesToSend)
        {
            var data = _orderProcessingRequestService.GetNotificationData(orderProcessingRequest.Id);

            return string.Join(Environment.NewLine + Environment.NewLine,
                               new[]
                                   {
                                       GetHeader(orderProcessingRequest, data),
                                       GetNotificationData(orderProcessingRequest, data),
                                       GetDescription(orderProcessingRequest),
                                       GetRenewedOrderDescription(orderProcessingRequest, data),
                                       GetMessages(messagesToSend),
                                       GetFooter(orderProcessingRequest)
                                   }
                                   .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string GetFooter(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            return orderProcessingRequest.RenewedOrderId == null && orderProcessingRequest.RequestType == (int)OrderProcessingRequestType.CreateOrder
                       ? BLResources.OrderProcessingRequestCreationResultEmailFooter
                       : null;
        }

        private static string GetHeader(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, OrderProcessingRequestNotificationData data)
        {
            return orderProcessingRequest.RequestType == (int)OrderProcessingRequestType.CreateOrder
                       ? BLResources.OrderProcessingRequestCreationResultEmailHeader
                       : string.Format(BLResources.OrderProcessingRequestProlongationResultEmailHeaderTemplate, data.BaseOrderNumber);
        }

        private static string GetNotificationData(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, OrderProcessingRequestNotificationData data)
        {
            return string.Format(BLResources.OrderProcessingRequestResultEmailDataTemplate,
                                 data.FirmName,
                                 data.LegalPersonName,
                                 orderProcessingRequest.BeginDistributionDate.ToShortDateString(),
                                 orderProcessingRequest.ReleaseCountPlan);
        }

        private static string GetDescription(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            return string.IsNullOrWhiteSpace(orderProcessingRequest.Description) 
                ? null : 
                string.Format(BLResources.OrderProcessingRequestResultEmailDescriptionTemplate, orderProcessingRequest.Description);
        }

        private static string GetRenewedOrderDescription(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, OrderProcessingRequestNotificationData data)
        {
            return orderProcessingRequest.RenewedOrderId == null
                       ? null
                       : string.Format(BLResources.OrderProcessingRequestResultEmailRenewedOrderTemplate, data.RenewedOrderNumber);
        }

        private static string GetMessages(IEnumerable<IMessageWithType> messagesToSend)
        {
            var messages = string.Join(
                Environment.NewLine, 
                messagesToSend.Where(x => AllowedMessageTypes.Contains(x.Type)).Select(FormatOrderProcessingRequestMessage));

            if (string.IsNullOrWhiteSpace(messages))
            {
                return null;
            }

            return string.Format(BLResources.OrderProcessingRequestResultEmailErrorsEnumerationTemplate, messages);
        }

        private static string FormatOrderProcessingRequestMessage(IMessageWithType message, int index)
        {
            return string.Format("{0}. {1}{2}.", index + 1, GetMessageTypePrefix(message.Type), message.MessageText);
        }

        private static string GetMessageTypePrefix(MessageType type)
        {
            return type == MessageType.Error
                       ? BLResources.OrderProcessingRequestResultEmailErrorMessagePrefixTemplate
                       : string.Empty;
        }
    }
}