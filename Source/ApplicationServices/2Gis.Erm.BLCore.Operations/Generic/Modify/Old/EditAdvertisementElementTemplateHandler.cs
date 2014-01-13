using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementElementTemplateHandler : RequestHandler<EditRequest<AdvertisementElementTemplate>, EmptyResponse>
    {
        private readonly IAdvertisementRepository _advertisementRepository;

        public EditAdvertisementElementTemplateHandler(IAdvertisementRepository advertisementRepository)
        {
            _advertisementRepository = advertisementRepository;
        }

        protected override EmptyResponse Handle(EditRequest<AdvertisementElementTemplate> request)
        {
            var advertisementElementTemplate = request.Entity;
            _advertisementRepository.CreateOrUpdate(advertisementElementTemplate);

            return Response.Empty;
        }
    }
}