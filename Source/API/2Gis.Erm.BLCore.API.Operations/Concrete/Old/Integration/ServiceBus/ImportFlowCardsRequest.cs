using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class ImportFlowCardsRequest : Request
    {
        public string BasicLanguage { get; set; }
        public string ReserveLanguage { get; set; }
        public int PregeneratedIdsAmount { get; set; }
        public string RegionalTerritoryLocaleSpecificWord { get; set; }
    }
}
