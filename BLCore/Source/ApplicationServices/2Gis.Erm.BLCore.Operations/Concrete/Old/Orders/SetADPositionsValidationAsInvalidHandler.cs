using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class SetADPositionsValidationAsInvalidHandler : RequestHandler<SetADPositionsValidationAsInvalidRequest, EmptyResponse>
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;

        public SetADPositionsValidationAsInvalidHandler(
            IFirmReadModel firmReadModel, 
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService)
        {
            _firmReadModel = firmReadModel;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
        }

        protected override EmptyResponse Handle(SetADPositionsValidationAsInvalidRequest request)
        {
            var firmId = _firmReadModel.GetOrderFirmId(request.OrderId);
            var firmNonArchivedOrderIds = _firmReadModel.GetFirmNonArchivedOrderIds(firmId);

            _registerOrderStateChangesOperationService.Changed(firmNonArchivedOrderIds.Select(x => new OrderChangesDescriptor
                                                                                                       {
                                                                                                           OrderId = x,
                                                                                                           ChangedAspects =
                                                                                                               new[]
                                                                                                                   {
                                                                                                                       OrderValidationRuleGroup
                                                                                                                           .ADPositionsValidation
                                                                                                                   }
                                                                                                       }));

            return Response.Empty;
        }
    }
}