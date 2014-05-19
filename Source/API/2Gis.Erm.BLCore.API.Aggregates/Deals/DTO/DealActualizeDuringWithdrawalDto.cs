namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public sealed class DealActualizeDuringWithdrawalDto
    {
        public DealActualizeProfitDto ActualizeProfitInfo { get; set; }
        public bool HasInactiveLocks { get; set; }
    }
}