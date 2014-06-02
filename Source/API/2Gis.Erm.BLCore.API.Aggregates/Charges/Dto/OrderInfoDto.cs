using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public sealed class OrderInfoDto
    {
        public OrderType OrderType { get; set; }
        public short ReleaseCountFact { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
    }
}