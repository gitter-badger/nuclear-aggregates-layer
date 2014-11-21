using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditOrderFileHandler : RequestHandler<EditRequest<OrderFile>, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public EditOrderFileHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(EditRequest<OrderFile> request)
        {
            _orderRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}
