using System;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public class OrderLinkingObjectsDto
    {
        public long FirmId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public short ReleaseCountFact { get; set; }
        public short ReleaseCountPlan { get; set; }
    }
}