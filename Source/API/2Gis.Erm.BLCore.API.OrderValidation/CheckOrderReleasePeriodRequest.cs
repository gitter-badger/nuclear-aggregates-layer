using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class CheckOrderReleasePeriodRequest : Request
    {
        public long OrderId { get; set; }
        public bool InProgressOnly { get; set; }
    }
}