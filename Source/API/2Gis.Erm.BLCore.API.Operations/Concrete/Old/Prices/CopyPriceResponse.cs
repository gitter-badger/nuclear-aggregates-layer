using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices
{
    public sealed class CopyPriceResponse : Response
    {
        public long TargetPriceId { get; set; }
    }
}