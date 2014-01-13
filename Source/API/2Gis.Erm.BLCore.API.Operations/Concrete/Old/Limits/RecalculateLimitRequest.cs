using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
// ReSharper restore CheckNamespace
{
    public sealed class RecalculateLimitRequest: Request
    {
        public long LimitId { get; set; }
    }
}
