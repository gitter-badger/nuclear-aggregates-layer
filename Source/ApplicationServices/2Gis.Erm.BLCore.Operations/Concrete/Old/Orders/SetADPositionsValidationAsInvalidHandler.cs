using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class SetADPositionsValidationAsInvalidHandler : RequestHandler<SetADPositionsValidationAsInvalidRequest, EmptyResponse>
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IOrderValidationInvalidator _orderValidationInvalidator;

        public SetADPositionsValidationAsInvalidHandler(
            IFirmRepository firmRepository,
            IOrderValidationInvalidator orderValidationInvalidator)
        {
            _firmRepository = firmRepository;
            _orderValidationInvalidator = orderValidationInvalidator;
        }

        protected override EmptyResponse Handle(SetADPositionsValidationAsInvalidRequest request)
        {
            var firmId = _firmRepository.GetOrderFirmId(request.OrderId);

            var firmNonArchivedOrderIds = _firmRepository.GetFirmNonArchivedOrderIds(firmId);
            _orderValidationInvalidator.Invalidate(firmNonArchivedOrderIds, OrderValidationRuleGroup.ADPositionsValidation);

            return Response.Empty;
        }
    }
}