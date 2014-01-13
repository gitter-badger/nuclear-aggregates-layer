using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Clients
{
    public sealed class MergeClientsRequest: Request
    {
        public long AppendedClientId { get; set; }
        public Client Client { get; set; }
    }
}
