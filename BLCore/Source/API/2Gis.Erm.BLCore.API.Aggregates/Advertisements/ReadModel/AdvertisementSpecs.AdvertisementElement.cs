using System;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel
{
    public static partial class AdvertisementSpecs
    {
        public static class AdvertisementElements
        {
            public static class Find
            {
                public static FindSpecification<AdvertisementElement> UnfilledDummyValuesForTemplate(long advertisementTemplateId)
                {
                    return new FindSpecification<AdvertisementElement>(x => !x.IsDeleted &&
                                                                            x.Advertisement.AdvertisementTemplateId == advertisementTemplateId &&
                                                                            x.Advertisement.FirmId == null &&
                                                                            ((x.BeginDate == null || x.EndDate == null) &&
                                                                             x.FileId == null &&
                                                                             String.IsNullOrEmpty(x.Text)));
                }
            }
        }
    }
}