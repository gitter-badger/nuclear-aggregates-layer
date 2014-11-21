using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public sealed class DealChangeStageDto
    {
        public Deal Deal { get; set; }
        public DealStage NextStage { get; set; }
    }
}