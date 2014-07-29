using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Core.Services.Notifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    // TODO {all, 17.12.2013}: Лучше разделить форматирование текста для операций создания заявки, реализации заявки и, возможно, других операций по заявке. 
    public interface IOrderProcessingRequestNotificationFormatter
    {
        NotificationMessage Format(OrderProcessingRequest orderProcessingRequest, IEnumerable<IMessageWithType> messagesToSend);
    }
}