namespace DoubleGis.Erm.BLCore.Aggregates.Deals.DTO
{
    public sealed class DealActualizeDuringWithdrawalDto
    {
        public DealActualizeProfitDto ActualizeProfitInfo { get; set; }
        public bool HasInactiveLocks { get; set; }
    }
}