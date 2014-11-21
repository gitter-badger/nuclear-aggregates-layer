using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum WithdrawalAggregate
    {
        WithdrawalInfo = EntityName.WithdrawalInfo,
        ReleaseWithdrawal = EntityName.ReleaseWithdrawal,
        ReleaseWithdrawalPosition = EntityName.ReleasesWithdrawalsPosition
    }
}
