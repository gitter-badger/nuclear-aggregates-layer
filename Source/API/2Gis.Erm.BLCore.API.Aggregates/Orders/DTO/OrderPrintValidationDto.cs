namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public sealed class OrderPrintValidationDto
    {
        public long? LegalPersonId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
    }
}
