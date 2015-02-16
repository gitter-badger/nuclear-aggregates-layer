using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO
{
    public sealed class OrderRecalculateWithdrawalsDto
    {
        public int LocksCount { get; set; }
        public IEnumerable<OrderReleaseTotal> ReleaseTotals { get; set; }
        public IEnumerable<OrderPositionDto> OrderPositions { get; set; }

        public sealed class OrderPositionDto
        {
            public long Id { get; set; }
            public decimal PayablePlan { get; set; }
            public decimal PayablePlanWoVat { get; set; }
            public bool IsComposite { get; set; }
            public long PlatformId { get; set; }
            public long OrderId { get; set; }
            public int Amount { get; set; }
            public decimal DiscountSum { get; set; }
            public decimal DiscountPercent { get; set; }
            public bool CalculateDiscountViaPercent { get; set; }
            public long PositionId { get; set; }
            public long PriceId { get; set; }
            public decimal CategoryRate { get; set; }
            
            public IEnumerable<OrderReleaseWithdrawalDto> ReleaseWithdrawals { get; set; }
        }
    }
}
