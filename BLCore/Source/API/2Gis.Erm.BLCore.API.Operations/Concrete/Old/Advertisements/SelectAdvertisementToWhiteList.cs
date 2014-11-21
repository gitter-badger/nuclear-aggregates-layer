using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements
{
    public sealed class SelectAdvertisementToWhiteListRequest : Request
    {
        public long AdvertisementId { get; set; }
        public long FirmId { get; set; }
    }
}