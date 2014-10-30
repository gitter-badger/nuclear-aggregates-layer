using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease
{
    public sealed class OrderInfo
    {
        public long Id { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string Number { get; set; }
        public long CuratorId { get; set; }
        public long? ApproverId { get; set; }
        public long? StableFirmId { get; set; }
        public OrderState Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public IEnumerable<OrderPositionInfo> Positions { get; set; }
        public IEnumerable<OrderPositionInfo> CompositePositions { get; set; }
    }
}