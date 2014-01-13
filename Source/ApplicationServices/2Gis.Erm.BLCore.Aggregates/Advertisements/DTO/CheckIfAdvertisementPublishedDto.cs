namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.DTO
{
    public sealed class CheckIfAdvertisementPublishedDto
    {
        public long AdvertisementId { get; set; }

        public bool IsPublished { get; set; }
        public bool IsDummy { get; set; }
    }
}
