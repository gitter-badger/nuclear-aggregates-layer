﻿using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class SetADPositionsValidationAsInvalidHandler : RequestHandler<SetADPositionsValidationAsInvalidRequest, EmptyResponse>
    {

        private readonly IOrderValidationInvalidator _orderValidationInvalidator;
        private readonly IFirmReadModel _firmReadModel;

        public SetADPositionsValidationAsInvalidHandler(IOrderValidationInvalidator orderValidationInvalidator, IFirmReadModel firmReadModel)
        {
            _orderValidationInvalidator = orderValidationInvalidator;
            _firmReadModel = firmReadModel;
        }

        protected override EmptyResponse Handle(SetADPositionsValidationAsInvalidRequest request)
        {
            var firmId = _firmReadModel.GetOrderFirmId(request.OrderId);

            var firmNonArchivedOrderIds = _firmReadModel.GetFirmNonArchivedOrderIds(firmId);
            _orderValidationInvalidator.Invalidate(firmNonArchivedOrderIds, OrderValidationRuleGroup.ADPositionsValidation);

            return Response.Empty;
        }
    }
}