using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO
{
    public class ChangeFirmTerritoryDto
    {
        public Firm Firm { get; set; }
        public long TerritoryId { get; set; }
    }
}