using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdditionalFirmServiceHandler : RequestHandler<EditRequest<AdditionalFirmService>, EmptyResponse>
    {
        private readonly IAdditionalFirmServicesService _additionalFirmServicesService;

        public EditAdditionalFirmServiceHandler(IAdditionalFirmServicesService additionalFirmServicesService)
        {
            _additionalFirmServicesService = additionalFirmServicesService;
        }

        protected override EmptyResponse Handle(EditRequest<AdditionalFirmService> request)
        {
            _additionalFirmServicesService.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}