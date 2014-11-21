namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class OrderValidationAdditionalInfo
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public string OwnerName { get; set; }
        public string FirmName { get; set; }
        public string LegalPersonName { get; set; }
    }
}
