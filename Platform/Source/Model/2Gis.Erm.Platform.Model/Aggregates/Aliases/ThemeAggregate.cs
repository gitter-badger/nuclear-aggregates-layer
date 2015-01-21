using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class ThemeAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Theme(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.ThemeTemplate(),
                                    EntityType.Instance.ThemeCategory(),
                                    EntityType.Instance.ThemeOrganizationUnit(),
                                    EntityType.Instance.FileWithContent()
                                })
                    .ToArray();
            }
        }
    }
}
