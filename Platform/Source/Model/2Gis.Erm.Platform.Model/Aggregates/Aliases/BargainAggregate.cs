using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class BargainAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Bargain(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.BargainFile(),
                                    EntityType.Instance.FileWithContent()
                                })
                    .ToArray();
            }
        }
    }
}
