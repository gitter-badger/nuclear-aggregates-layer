using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class SelectPrintProfilesOperationService : ISelectPrintProfilesOperationService
    {
        private readonly IOrderReadModel _orderReadModel;

        public SelectPrintProfilesOperationService(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        public OrderProfiles SelectProfilesByBill(long billId)
        {
            var order = _orderReadModel.GetOrderByBill(billId);
            return SelectProfiles(order);
        }

        public OrderProfiles SelectProfilesByOrder(long orderId)
        {
            var order = _orderReadModel.GetOrder(orderId);
            return SelectProfiles(order);
        }

        private OrderProfiles SelectProfiles(Order order)
        {
            if (order.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            return new OrderProfiles { LegalPersonProfileId = order.LegalPersonProfileId.Value };
        }
    }
}
