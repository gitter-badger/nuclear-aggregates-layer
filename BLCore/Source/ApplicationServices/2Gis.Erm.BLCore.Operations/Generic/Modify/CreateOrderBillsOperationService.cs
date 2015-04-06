using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class CreateOrderBillsOperationService : ICreateOrderBillsOperationService
    {
        private readonly BillFactory _billFactory;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBulkDeleteBillAggregateService _deleteAggregateService;
        private readonly IBulkCreateBillAggregateService _createAggregateService;

        public CreateOrderBillsOperationService(
            IBulkCreateBillAggregateService createAggregateService,
            IOrderReadModel orderReadModel,
            IBulkDeleteBillAggregateService deleteAggregateService,
            IOperationScopeFactory scopeFactory,
            BillFactory billFactory)
        {
            _createAggregateService = createAggregateService;
            _orderReadModel = orderReadModel;
            _deleteAggregateService = deleteAggregateService;
            _scopeFactory = scopeFactory;
            _billFactory = billFactory;
        }

        public void Create(long orderId, CreateBillInfo[] createBillInfos)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);
            var bills = _billFactory.Create(order, createBillInfos);
            SaveBills(order, bills);
        }

        private void SaveBills(Order order, IEnumerable<Bill> bills)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CreateOrderBillsIdentity>())
            {
                // delete previous bills
                var oldBills = _orderReadModel.GetBillsForOrder(order.Id);
                _deleteAggregateService.DeleteBills(order, oldBills);
                _createAggregateService.Create(order, bills);

                scope.Deleted(oldBills)
                     .Added(bills)
                     .Updated(order)
                     .Complete();
            }
        }
    }
}