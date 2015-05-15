using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class LegalPersonAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.LegalPerson(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.Account(), //
                                    EntityType.Instance.Bargain(), //
                                    EntityType.Instance.Order(), //
                                    EntityType.Instance.Limit(), //
                                    EntityType.Instance.Limit(), //
                                    EntityType.Instance.LegalPersonProfile()
                                })
                    .ToArray();
            }
        }
    }
}
