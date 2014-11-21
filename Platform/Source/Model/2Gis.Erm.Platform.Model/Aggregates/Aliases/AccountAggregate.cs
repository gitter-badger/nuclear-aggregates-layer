using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum AccountAggregate
    {
        Account = EntityName.Account,
        AccountDetail = EntityName.AccountDetail,
        Limit = EntityName.Limit,
        Lock = EntityName.Lock, 
        LockDetail = EntityName.LockDetail,
        OperationType = EntityName.OperationType,
        WitdrawalInfo = EntityName.WithdrawalInfo
    }
}
