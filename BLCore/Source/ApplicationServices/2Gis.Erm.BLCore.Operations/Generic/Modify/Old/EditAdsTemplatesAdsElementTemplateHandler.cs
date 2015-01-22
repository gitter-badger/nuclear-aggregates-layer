using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdsTemplatesAdsElementTemplateHandler : RequestHandler<EditRequest<AdsTemplatesAdsElementTemplate>, EmptyResponse>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public EditAdsTemplatesAdsElementTemplateHandler(
            IAdvertisementRepository advertisementRepository,
            IOperationScopeFactory operationScopeFactory)
        {
            _advertisementRepository = advertisementRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<AdsTemplatesAdsElementTemplate> request)
        {
            var adsTemplatesAdsElementTemplate = request.Entity;

            var isAdvertisementTemplatePublished = _advertisementRepository.IsAdvertisementTemplatePublished(request.Entity.AdsTemplateId);
            if (isAdvertisementTemplatePublished)
            {
                throw new BusinessLogicException(BLResources.CanNotChangePublishedAdvertisementTemplate);
            }

            if (adsTemplatesAdsElementTemplate.IsNew())
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<AppendIdentity, AdvertisementTemplate, AdvertisementElementTemplate>())
                {
                    _advertisementRepository.Create(adsTemplatesAdsElementTemplate);
                    _advertisementRepository.AddAdvertisementsElementsFromTemplate(adsTemplatesAdsElementTemplate);

                    operationScope.Updated<AdvertisementTemplate>(adsTemplatesAdsElementTemplate.AdsTemplateId)
                                  .Updated<AdvertisementElementTemplate>(adsTemplatesAdsElementTemplate.AdsElementTemplateId);
                    operationScope.Complete();
                }
            }
            else
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<AppendIdentity, AdvertisementTemplate, AdvertisementElementTemplate>())
                {
                    _advertisementRepository.Update(adsTemplatesAdsElementTemplate);

                    operationScope.Updated<AdvertisementTemplate>(adsTemplatesAdsElementTemplate.AdsTemplateId)
                                  .Updated<AdvertisementElementTemplate>(adsTemplatesAdsElementTemplate.AdsElementTemplateId);
                    operationScope.Complete();
                }
            }

            return Response.Empty;
        }
    }
}