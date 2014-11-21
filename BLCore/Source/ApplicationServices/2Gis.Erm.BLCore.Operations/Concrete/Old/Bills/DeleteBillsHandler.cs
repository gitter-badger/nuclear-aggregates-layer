using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class DeleteBillsHandler : RequestHandler<DeleteBillsRequest, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;

        public DeleteBillsHandler(
            IOrderRepository orderRepository,
            IOrderReadModel orderReadModel)
        {
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
        }

        protected override EmptyResponse Handle(DeleteBillsRequest request)
        {
            var orderWithBills = _orderReadModel.GetOrderWithBills(request.OrderId);
            var isOrderOnApproval = orderWithBills.Order != null && orderWithBills.Order.WorkflowStepId == (int)OrderState.OnRegistration;

            if (!isOrderOnApproval)
            {
                throw new NotificationException(BLResources.CantEditBillsWhenOrderIsNotOnRegistration);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var bill in orderWithBills.Bills)
                {
                    _orderRepository.Delete(bill);
                }

                transaction.Complete();
            }

            return Response.Empty;
        }
    }
}