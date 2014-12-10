using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class DeleteOrderBillsOperationService : IDeleteOrderBillsOperationService
    {
        private readonly IDeleteBillsAggregateService _deleteBillsService;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteOrderBillsOperationService(
            IDeleteBillsAggregateService deleteBillsService,
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

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Bill>())
            {
                _deleteBillsService.DeleteBills(order, bills);
                scope.Deleted(bills)
                     .Complete();
            }
        }
    }
}