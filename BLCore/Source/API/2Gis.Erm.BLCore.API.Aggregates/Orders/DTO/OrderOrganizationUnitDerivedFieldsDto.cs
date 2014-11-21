using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public sealed class OrderOrganizationUnitDerivedFieldsDto
    {
        public EntityReference OrganizationUnit { get; set; }
        public EntityReference Currency { get; set; }
    }
}