using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class OrderAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Order(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.OrderPosition(),
                                    EntityType.Instance.OrderPositionAdvertisement(),
                                    EntityType.Instance.Bill(), //
                                    EntityType.Instance.OrderFile(),
                                    EntityType.Instance.FileWithContent(),
                                    EntityType.Instance.OrderReleaseTotal(),
                                    EntityType.Instance.Bargain(),
                                    EntityType.Instance.ReleaseWithdrawal(),
                                    EntityType.Instance.ReleasesWithdrawalsPosition()
                                })
                    .ToArray();
            }
        }
    }
}
