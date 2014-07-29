using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public class ClientAndFirmForDealInfo
    {
        public Client Client { get; set; }
        public Firm MainFirm { get; set; }
    }
}