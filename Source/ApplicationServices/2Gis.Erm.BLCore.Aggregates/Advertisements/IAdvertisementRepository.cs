using DoubleGis.Erm.BLCore.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements
{
    public interface IAdvertisementRepository : IAggregateRootRepository<Advertisement>,
                                                IDeleteAggregateRepository<Advertisement>,
                                                IDeleteAggregateRepository<AdvertisementTemplate>,
                                                IDeleteAggregateRepository<AdvertisementElement>,
                                                IDeleteAggregateRepository<AdvertisementElementTemplate>,
                                                IDeleteAggregateRepository<AdsTemplatesAdsElementTemplate>,
                                                IDownloadFileAggregateRepository<AdvertisementElement>
    {
        int Delete(Advertisement entity);
        int Delete(AdvertisementTemplate entity);
        int Delete(AdvertisementElement entity);
        int Delete(AdvertisementElementTemplate entity);
        int Delete(AdsTemplatesAdsElementTemplate entity);

        void CreateOrUpdate(Advertisement advertisement);
        void SelectToWhiteList(long firmId, long advertisementId);
        AdvertisementBagItem[] GetAdvertisementBag(long advertisementId);
        bool IsAdvertisementTemplatePublished(long advertisementTemplateId);
        bool IsAdvertisementTemplateTheSameInAdvertisementAndElements(long advertisementTemplateId, long advertisementId);
        AdsTemplatesAdsElementTemplate GetAdsTemplatesAdsElementTemplate(long entityId);
        AdvertisementElementTemplate GetAdvertisementElementTemplate(long entityId);
        Advertisement GetSelectedToWhiteListAdvertisement(long firmId);

        void CreateOrUpdate(AdvertisementTemplate advertisementTemplate);
        void CreateOrUpdate(AdvertisementElementTemplate advertisementElementTemplate);

        void Create(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate);
        void Update(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate);

        void AddAdvertisementsElementsFromTemplate(AdsTemplatesAdsElementTemplate adsTemplatesAdsElementTemplate);
        void AddAdvertisementsElementsFromAdvertisement(Advertisement advertisement);
        void Publish(long advertisementTemplateId);
        void Unpublish(long advertisementTemplateId);

        AdvertisementTemplateIdNameDto GetAdvertisementTemplate(long advertisementId);
    }
}
