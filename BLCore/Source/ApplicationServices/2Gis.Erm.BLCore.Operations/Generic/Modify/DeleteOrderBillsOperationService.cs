using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class DeleteOrderBillsOperationService : IDeleteOrderBillsOperationService
    {
        private readonly IBulkDeleteBillAggregateService _deleteBillsService;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteOrderBillsOperationService(
            IBulkDeleteBillAggregateService deleteBillsService,
            IOrderReadModel orderReadModel,
            IOperationScopeFactory operationScopeFactory)
        {
            _deleteBillsService = deleteBillsService;
            _orderReadModel = orderReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(long orderId)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);
            var bills = _orderReadModel.GetBillsForOrder(orderId);

            using (var scope = _operationScopeFactory.CreateNonCoupled<DeleteOrderBillsIdentity>())
            {
                _deleteBillsService.DeleteBills(order, bills);

                scope.Deleted(bills)
                     .Updated(order)
                     .Complete();
            }
        }
    }
}