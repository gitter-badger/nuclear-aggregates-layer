namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    public class ImportFirmAddressDto
    {
        public long Code { get; set; }
        public long FirmCode { get; set; }
        public int BranchCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsLinked { get; set; }
        public bool IsDeleted { get; set; }

        public string Address { get; set; }
        public long? TerritoryCode { get; set; }
        public string Schedule { get; set; }
        public string Payment { get; set; }
        public bool ClosedForAscertainment { get; set; }
    }
}