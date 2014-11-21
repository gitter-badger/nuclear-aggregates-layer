using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO
{
    public class ChangeClientTerritoryDto
    {
        public Client Client { get; set; }
        public long TerritoryId { get; set; }
    }
}