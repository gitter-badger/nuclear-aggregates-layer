using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBillHandler : RequestHandler<EditRequest<Bill>, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;
        
        public EditBillHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Bill> request)
        {
            var bill = request.Entity;
            _orderRepository.CreateOrUpdate(bill);

            return Response.Empty;
        }
    }
}