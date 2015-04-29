using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders
{
    public interface IOrderRepository : IAggregateRootRepository<Order>,
                                        IAssignAggregateRepository<Order>,
                                        IDeleteAggregateRepository<OrderPosition>,
                                        IUploadFileAggregateRepository<OrderFile>,
                                        IDownloadFileAggregateRepository<OrderFile>
    {
        void CloseOrder(Order order, string reason);

        int Create(Order order);

        int CreateOrUpdate(OrderFile entity);

        int Update(Order order);

        int CreateOrUpdate(OrderPosition orderPosition);

        int Update(OrderPosition orderPosition);

        int Assign(Order order, long ownerCode);

        int Delete(OrderPosition orderPosition);

        int Delete(IEnumerable<OrderPositionAdvertisement> advertisements);

        void SetInspector(long orderId, long? inspectorId);

        int SetOrderState(Order order, OrderState orderState);

        long GenerateNextOrderUniqueNumber();

        Order CreateCopiedOrder(Order order, IEnumerable<OrderPositionWithAdvertisementsDto> orderPositionDtos);
    }
}