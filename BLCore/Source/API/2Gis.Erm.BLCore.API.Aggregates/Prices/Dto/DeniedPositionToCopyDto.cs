using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class DeniedPositionToCopyDto
    {
        public long PositionId { get; set; }
        public long PositionDeniedId { get; set; }
        public ObjectBindingType ObjectBindingType { get; set; }
    }
}
