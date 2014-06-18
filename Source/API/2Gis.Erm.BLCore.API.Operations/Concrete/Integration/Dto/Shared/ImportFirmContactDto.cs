namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared
{
    public class ImportFirmContactDto
    {
        public long? FirmAddressId { get; set; }
        public long? CardId { get; set; }
        public int ContactType { get; set; }
        public string Contact { get; set; }
        public int SortingPosition { get; set; }
    }
}