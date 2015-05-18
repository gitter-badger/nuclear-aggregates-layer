using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public static class AccountAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Account(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.AccountDetail(),
                                    EntityType.Instance.Limit(),
                                    EntityType.Instance.Lock(),
                                    EntityType.Instance.LockDetail(),
                                    EntityType.Instance.OperationType(),
                                    EntityType.Instance.WithdrawalInfo()
                                })
                    .ToArray();
            }
        }
    }
}
