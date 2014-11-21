using System;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public sealed class OrderWithDummyAdvertisementDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public string OwnerName { get; set; }
        public string FirmName { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public string WorkflowStep { get; set; }
    }
}