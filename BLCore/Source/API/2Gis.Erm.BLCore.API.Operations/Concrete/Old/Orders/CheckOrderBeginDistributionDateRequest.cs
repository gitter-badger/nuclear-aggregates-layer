using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class CheckOrderBeginDistributionDateRequest : Request
    {
        public long OrderId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestinationOrganizationUnitId { get; set; }
    }
}