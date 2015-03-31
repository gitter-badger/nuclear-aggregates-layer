namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.DTO
{
    public sealed class CategoryAsLinkingObjectDto
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryLevel { get; set; }
        public long PositionId { get; set; }
        public long? FirmAddressId { get; set; }
    }
}