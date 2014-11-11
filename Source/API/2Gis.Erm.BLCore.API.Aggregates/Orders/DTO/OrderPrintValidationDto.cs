namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public class OrderPrintValidationDto
    {
        public long? LegalPersonId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
    }

    public class OrderForBargainPrintValidationDto : OrderPrintValidationDto
    {
        public long? BargainId { get; set; }
    }
}
