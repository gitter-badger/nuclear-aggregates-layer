using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class OrganizationUnitAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.OrganizationUnit(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root };
            }
        }
    }
}