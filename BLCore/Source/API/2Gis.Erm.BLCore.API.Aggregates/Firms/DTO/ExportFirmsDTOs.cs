using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO
{
    public sealed class FirmDto
    {
        public long Id { get; set; }
        public IEnumerable<OrderDto> OrderDtos { get; set; }
    }

    public sealed class OrderDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
    }

    public sealed class OrganizationUnitDto
    {
        public long Id { get; set; }
        public int DgppId { get; set; }
        public string Name { get; set; }

        public IEnumerable<FirmDto> FirmDtos { get; set; }
    }

    public sealed class FirmAndClientDto
    {
        public Firm Firm { get; set; }
        public Client Client { get; set; }
    }
}