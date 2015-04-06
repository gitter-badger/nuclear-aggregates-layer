using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public sealed class OrderRelationsDto
    {
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public string OrderNumber { get; set; }
        public short CurrencyIsoCode { get; set; }
        public long? LegalPersonId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long FirmId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public long BranchOfficeId { get; set; }
        public SalesModel? SalesModel { get; set; }
    }
}
