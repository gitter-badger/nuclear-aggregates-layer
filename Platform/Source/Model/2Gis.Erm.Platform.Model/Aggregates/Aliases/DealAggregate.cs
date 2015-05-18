using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class DealAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Deal(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
    {
                                    EntityType.Instance.Order(),
                                    EntityType.Instance.OrderPosition(),
                                    EntityType.Instance.FirmDeal(),
                                    EntityType.Instance.LegalPersonDeal()
                                })
                    .ToArray();
            }
        }
    }
}
