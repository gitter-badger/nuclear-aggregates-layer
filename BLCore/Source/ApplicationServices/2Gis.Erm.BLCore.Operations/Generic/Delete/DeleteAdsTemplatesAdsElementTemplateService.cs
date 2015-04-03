using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteAdsTemplatesAdsElementTemplateService : IDeleteGenericEntityService<AdsTemplatesAdsElementTemplate>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteAdsTemplatesAdsElementTemplateService(IAdvertisementRepository advertisementRepository, IOperationScopeFactory operationScopeFactory)
        {
            _advertisementRepository = advertisementRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var adsTemplatesAdsElementTemplate = _advertisementRepository.GetAdsTemplatesAdsElementTemplate(entityId);
            var isAdvertisementTemplatePublished = _advertisementRepository.IsAdvertisementTemplatePublished(adsTemplatesAdsElementTemplate.AdsTemplateId);
            if (isAdvertisementTemplatePublished)
            {
                throw new BusinessLogicException(BLResources.CanNotChangePublishedAdvertisementTemplate);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DetachIdentity, AdvertisementTemplate, AdvertisementElementTemplate>())
            {
                var deleteAggregateRepository = (IDeleteAggregateRepository<AdsTemplatesAdsElementTemplate>)_advertisementRepository;
                deleteAggregateRepository.Delete(entityId);

                operationScope.Updated<AdvertisementTemplate>(adsTemplatesAdsElementTemplate.AdsTemplateId)
                              .Updated<AdvertisementElementTemplate>(adsTemplatesAdsElementTemplate.AdsElementTemplateId);
                operationScope.Complete();
            }

            return null;
        }

        // not used
        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}