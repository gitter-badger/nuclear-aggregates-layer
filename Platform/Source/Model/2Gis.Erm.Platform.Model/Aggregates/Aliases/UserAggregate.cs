using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class UserAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.User(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.UserRole(),
                                    EntityType.Instance.UserOrganizationUnit(),
                                    EntityType.Instance.UserTerritory(),
                                    EntityType.Instance.Department(),
                                    EntityType.Instance.FileWithContent(),
                                    EntityType.Instance.UserProfile(),
                                    EntityType.Instance.OrganizationUnit(), //
                                    EntityType.Instance.Territory(), //
                                    EntityType.Instance.Client(), //
                                    EntityType.Instance.Firm(), //
                                    EntityType.Instance.Deal(), //
                                    EntityType.Instance.LegalPerson(), //
                                    EntityType.Instance.LegalPersonProfile(), //
                                    EntityType.Instance.Bargain(), //
                                    EntityType.Instance.Contact(), //
                                    EntityType.Instance.Order(), //
                                    EntityType.Instance.OrderPosition(), //
                                    EntityType.Instance.Account(), //
                                    EntityType.Instance.Limit(), //
                                })
                    .ToArray();
            }
        }
    }
}
