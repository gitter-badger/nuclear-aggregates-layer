using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.PricePositions
{
    public class CopyPricePositionRequest : Request
    {
        public long PriceId { get; set; }
        public long SourcePricePositionId { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
    }
}