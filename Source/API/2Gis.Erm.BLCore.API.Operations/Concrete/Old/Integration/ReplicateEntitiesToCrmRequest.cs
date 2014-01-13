using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration
{
    public class ReplicateEntitiesToCrmRequest : Request
    {
        public int Timeout { get; set; }
        public int ChunkSize { get; set; }
    }
}
