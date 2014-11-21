using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders
{
    public interface IOrderRepository : IAggregateRootRepository<Order>,
                                        IAssignAggregateRepository<Order>,
                                        IDeleteAggregateRepository<Bill>,
                                        IDeleteAggregateRepository<OrderPosition>,
                                        IUploadFileAggregateRepository<OrderFile>,
                                        IDownloadFileAggregateRepository<OrderFile>
    {
        void CloseOrder(Order order, string reason);

        int Create(Order order);

        int CreateOrUpdate(Bill bill);

        int CreateOrUpdate(OrderFile entity);

        int Update(Order order);

        int CreateOrUpdate(OrderPosition orderPosition);

        int Update(OrderPosition orderPosition);

        int Assign(Order order, long ownerCode);

        int Delete(OrderPosition orderPosition);

        int Delete(Bill bill);

        void CreateOrUpdateOrderPositionAdvertisements(long orderPositionId, AdvertisementDescriptor[] newAdvertisementsLinks, bool orderIsLocked);

        int Delete(IEnumerable<OrderPositionAdvertisement> advertisements);

        void UpdateOrderNumber(Order order);

        void SetInspector(long orderId, long? inspectorId);

        int SetOrderState(Order order, OrderState orderState);

        void ChangeOrderPositionBindingObjects(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements);

        long GenerateNextOrderUniqueNumber();

        Order CreateCopiedOrder(Order order, IEnumerable<OrderPositionWithAdvertisementsDto> orderPositionDtos);

        // Удаляет объекты OrderReleaseTotal, имеющие отношение к заказу и возвращает идентификаторы удалённых объектов
        long[] DeleteOrderReleaseTotalsForOrder(long orderId);

        void CreateOrderReleaseTotals(IEnumerable<OrderReleaseTotal> orderReleaseTotals);
    }
}