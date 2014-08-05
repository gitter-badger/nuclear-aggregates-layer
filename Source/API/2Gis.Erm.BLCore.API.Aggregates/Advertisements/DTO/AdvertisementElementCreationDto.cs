namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementElementCreationDto
    {
        public long AdsTemplatesAdsElementTemplateId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }
        public bool IsFasComment { get; set; }
        public bool NeedsValidation { get; set; }
        public bool IsRequired { get; set; }
    }
}
