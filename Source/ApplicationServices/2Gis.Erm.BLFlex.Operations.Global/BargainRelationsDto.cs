namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public sealed class BargainRelationsDto
    {
        public string BargainNumber { get; set; }
        public short CurrencyIsoCode { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public long? LegalPersonId { get; set; }
    }
}