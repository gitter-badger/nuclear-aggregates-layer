using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class ImportFlowRubricsRequest : Request
    {
        public string BasicLanguage { get; set; }
        public string ReserveLanguage { get; set; }
    }
}
