using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements
{
    public static class AdvertisementSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Advertisement> AdvertisementById(long id)
            {
                return new FindSpecification<Advertisement>(x => x.Id == id);
            }

            public static FindSpecification<AdvertisementTemplate> AdvertisementTemplateById(long id)
            {
                return new FindSpecification<AdvertisementTemplate>(x => x.Id == id);
            }

            public static FindSpecification<AdvertisementElement> AdvertisementElementById(long id)
            {
                return new FindSpecification<AdvertisementElement>(x => x.Id == id);
            }

            public static FindSpecification<AdvertisementElement> UnfilledDummyValuesForTemplate(long advertisementTemplateId)
            {
                return new FindSpecification<AdvertisementElement>(x => !x.IsDeleted &&
                                                                        x.Advertisement.AdvertisementTemplateId == advertisementTemplateId &&
                                                                        x.Advertisement.FirmId == null &&
                                                                        ((x.BeginDate == null || x.EndDate == null) &&
                                                                         x.FileId == null &&
                                                                         string.IsNullOrEmpty(x.Text)));
            }

            public static FindSpecification<AdvertisementElementTemplate> AdvertisementElementTemplateById(long id)
            {
                return new FindSpecification<AdvertisementElementTemplate>(x => x.Id == id);
            }

            public static FindSpecification<AdsTemplatesAdsElementTemplate> AdsTemplatesAdsElementTemplateById(long id)
            {
                return new FindSpecification<AdsTemplatesAdsElementTemplate>(x => x.Id == id);
            }
        }
    }
}
