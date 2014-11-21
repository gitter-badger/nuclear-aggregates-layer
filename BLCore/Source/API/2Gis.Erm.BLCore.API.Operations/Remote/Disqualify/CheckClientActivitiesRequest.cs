using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify
{
    public sealed class CheckClientActivitiesRequest : Request
    {
        public long ClientId { get; set; }
    }
}