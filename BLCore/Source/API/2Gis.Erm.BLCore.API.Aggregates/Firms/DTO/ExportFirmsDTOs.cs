using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO
{
    public sealed class FirmAndClientDto
    {
        public Firm Firm { get; set; }
        public Client Client { get; set; }
    }
}