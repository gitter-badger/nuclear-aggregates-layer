using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class ClientAggregate
    { 
        public static IEntityType Root
        {
            get { return EntityType.Instance.Client(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.Firm(), //
                                    EntityType.Instance.LegalPerson(), //
                                    EntityType.Instance.LegalPersonProfile(), //
                                    EntityType.Instance.Account(), //
                                    EntityType.Instance.Limit(), //
                                    EntityType.Instance.Bargain(), //
                                    EntityType.Instance.Deal(), //
                                    EntityType.Instance.Order(), //
                                    EntityType.Instance.OrderPosition(), //
                                    EntityType.Instance.Contact(),
                                    EntityType.Instance.ClientLink(), //
                                    EntityType.Instance.DenormalizedClientLink(), //

                                    // FIXME {s.pomadin, 24.09.2014}: remove after merge with AM branch
                                    EntityType.Instance.Appointment(),
                                    EntityType.Instance.Phonecall(),
                                    EntityType.Instance.Task()
                                })
                    .ToArray();
            }
        }
    }
}
