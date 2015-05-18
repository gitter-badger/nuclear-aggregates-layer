using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class FirmAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Firm(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.FirmAddress(),
                                    EntityType.Instance.FirmContact(),
                                    EntityType.Instance.Client(),
                                    EntityType.Instance.CategoryFirmAddress(),
                                    EntityType.Instance.CityPhoneZone(),
                                    EntityType.Instance.Reference(),
                                    EntityType.Instance.ReferenceItem(),
                                    EntityType.Instance.CardRelation(),
                                    EntityType.Instance.Territory(),
                                    EntityType.Instance.DepCard(),
                                    EntityType.Instance.HotClientRequest()
                                })
                    .ToArray();
            }
        }
    }
}
