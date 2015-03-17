using DoubleGis.Erm.BL.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.Operations.Concrete.Orders
{
    public class GetOrderDocumentsDebtInfoOperationService : IGetOrderDocumentsDebtInfoOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public GetOrderDocumentsDebtInfoOperationService(IOrderReadModel orderReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _orderReadModel = orderReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public OrderDocumentsDebtDto GetOrderDocumentsDebtInfo(long orderId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetOrderDocumentsDebtInfoIdentity>())
            {
                var result = _orderReadModel.GetOrderDocumentsDebtInfo(orderId);
                scope.Complete();
                return result;
            }
        }
    }
}