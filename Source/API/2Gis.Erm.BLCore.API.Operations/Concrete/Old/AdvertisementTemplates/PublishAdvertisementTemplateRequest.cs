using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates
{
    public sealed class PublishAdvertisementTemplateRequest : Request
    {
        public long AdvertisementTemplateId { get; set; }
    }
}
