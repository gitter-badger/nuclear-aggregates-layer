namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared
{
    public class ImportCategoryFirmAddressDto
    {
        public long FirmAddressId { get; set; }
        public long CategoryId { get; set; }
        public bool IsPrimary { get; set; }
        public int SortingPosition { get; set; }
    }
}