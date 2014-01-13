using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices
{
    public sealed class ReplacePriceRequest : Request
    {
        public long SourcePriceId { get; set; }
        public long TargetPriceId { get; set; }
    }
}