using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class AdvertisementAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Advertisement(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.AdvertisementElement(),
                                    EntityType.Instance.AdvertisementElementStatus(),
                                    EntityType.Instance.AdvertisementElementDenialReason(),
                                    EntityType.Instance.AdvertisementTemplate(),
                                    EntityType.Instance.AdvertisementElementTemplate(),
                                    EntityType.Instance.AdsTemplatesAdsElementTemplate(),
                                    EntityType.Instance.FileWithContent()
                                })
                    .ToArray();
            }
        }
    }
}
