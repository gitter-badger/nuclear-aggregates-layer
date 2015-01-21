using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class PriceAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Price(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.PricePosition(),
                                    EntityType.Instance.AssociatedPositionsGroup(),
                                    EntityType.Instance.AssociatedPosition(),
                                    EntityType.Instance.DeniedPosition()
                                })
                    .ToArray();
            }
        }
    }
}
