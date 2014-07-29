using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public sealed class DealActualizeProfitDto
    {
        public Deal Deal { get; set; }
        public decimal RemainingExpectedProfit { get; set; }
        public decimal? ActuallyReceivedProfit { get; set; }
    }
}