using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class DeniedPositionToCreateDto
    {
        public long PositionDeniedId { get; set; }
        public ObjectBindingType ObjectBindingType { get; set; }
    }
}
