using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class DeleteOrderBillsOperationService : IDeleteOrderBillsOperationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public void Delete(long orderId)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);
            var bills = _orderReadModel.GetBillsForOrder(orderId);

            var isOrderOnApproval = order != null && order.WorkflowStepId == OrderState.OnRegistration;
            if (!isOrderOnApproval)
            {
                throw new NotificationException(BLResources.CantEditBillsWhenOrderIsNotOnRegistration);
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Bill>())
            {
                foreach (var bill in bills)
                {
                    _orderRepository.Delete(bill);
                }

                scope.Deleted(bills)
                     .Complete();
            }
        }
    }
}