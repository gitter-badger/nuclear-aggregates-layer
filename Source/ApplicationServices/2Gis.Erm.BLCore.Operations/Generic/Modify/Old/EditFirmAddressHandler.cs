using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditFirmAddressHandler : RequestHandler<EditRequest<FirmAddress>, EmptyResponse>
    {
        private readonly IUpdateAggregateRepository<FirmAddress> _updateFirmAddressService;

        public EditFirmAddressHandler(IUpdateAggregateRepository<FirmAddress> updateFirmAddressService)
        {
            _updateFirmAddressService = updateFirmAddressService;
        }

        protected override EmptyResponse Handle(EditRequest<FirmAddress> request)
        {
            _updateFirmAddressService.Update(request.Entity);
            return Response.Empty;
        }
    }
}