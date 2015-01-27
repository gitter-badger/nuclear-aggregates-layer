using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO
{
    public sealed class OrderReleaseWithdrawalDto
    {
        public ReleaseWithdrawal WidrawalInfo { get; set; }
        public IEnumerable<ReleasesWithdrawalsPosition> WithdrawalsPositions { get; set; }
    }
}