using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
{
    public sealed class RecalculateLimitRequest : Request
    {
        public long LimitId { get; set; }
    }
}
