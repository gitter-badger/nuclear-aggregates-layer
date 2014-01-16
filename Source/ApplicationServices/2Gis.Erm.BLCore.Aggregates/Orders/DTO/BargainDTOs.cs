using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.DTO
{
    public sealed class BargainUsageDto
    {
        public Bargain Bargain { get; set; }
        public IEnumerable<string> OrderNumbers { get; set; }
    }

    public sealed class CreateBargainInfo
    {
        public long? LegalPersonId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long? BargainTypeId { get; set; }
        public DateTime OrderSignupDate { get; set; }
        public Order Order { get; set; }
        public string DestinationSyncCode1C { get; set; }
        public string SourceSyncCode1C { get; set; }
    }
}