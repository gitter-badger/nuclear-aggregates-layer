using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public sealed class DealActualizeDuringWithdrawalDto
    {
        public Deal Deal { get; set; }
        public bool HasInactiveLocks { get; set; }
    }
}