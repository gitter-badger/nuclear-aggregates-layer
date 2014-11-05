using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public sealed class OrderParentEntityDerivedFieldsDto
    {
        public EntityReference Client { get; set; }
        public EntityReference Firm { get; set; }
        public EntityReference LegalPerson { get; set; }
        public EntityReference DestOrganizationUnit { get; set; }
        public EntityReference Owner { get; set; }
        public EntityReference Deal { get; set; }
        public EntityReference DealCurrency { get; set; }
    }
}
