using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class LocalMessageAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.LocalMessage(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.FileWithContent()
                                })
                    .ToArray();
            }
        }
    }
}
