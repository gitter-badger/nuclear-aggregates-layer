using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBillHandler : RequestHandler<EditRequest<Bill>, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidateBillsService _validateBillsService;

        public EditBillHandler(IOrderRepository orderRepository, IValidateBillsService validateBillsService)
        {
            _orderRepository = orderRepository;
            _validateBillsService = validateBillsService;
        }

        protected override EmptyResponse Handle(EditRequest<Bill> request)
        {
            var bill = request.Entity;

            string report;
            if (!_validateBillsService.PreValidate(new[] { bill }, out report) || !_validateBillsService.Validate(new[] { bill }, out report))
            {
                throw new NotificationException(report);
            }

            _orderRepository.CreateOrUpdate(bill);

            return Response.Empty;
        }
    }
}