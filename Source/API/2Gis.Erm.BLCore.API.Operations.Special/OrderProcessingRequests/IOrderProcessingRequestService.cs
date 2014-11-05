using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 05.12.2013}: Придётся распилить, часть ляжет в 2Gis.Erm.BLCore.Aggregates\Orders\ReadModel\IOrderProcessingRequestService.cs
    //                              а методы CreateOrUpdate, SaveMessagesForOrderProcessingRequest в OrderRepository (или в один из маленьких продолжателей его дела)
    public interface IOrderProcessingRequestService : ISimplifiedModelConsumer
    {
        void Create(OrderProcessingRequest orderProcessingRequest);
        
        void Update(OrderProcessingRequest orderProcessingRequest);

        void SaveMessagesForOrderProcessingRequest(long orderProcessingRequestId, IEnumerable<IMessageWithType> messages);

        // TODO {d.ivanov, 29.11.2013}: IReadOnlyModel
        IEnumerable<OrderProcessingRequest> GetProlongationRequestsToProcess();

        // TODO {d.ivanov, 29.11.2013}: IReadOnlyModel
        OrderProcessingRequest GetProlongationRequestToProcess(long id);

        // TODO {d.ivanov, 29.11.2013}: IReadOnlyModel
        OrderProcessingRequestFirmDto GetFirmDto(long firmId);

        // TODO {d.ivanov, 04.12.2013}: IReadOnlyModel
        OrderProcessingRequestOrderDto GetOrderDto(long orderId);

        // TODO {d.ivanov, 04.12.2013}: IReadOnlyModel
        OrderProcessingRequestNotificationData GetNotificationData(long orderProcessingRequestId);
    }
}
