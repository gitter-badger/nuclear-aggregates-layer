using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Billing
{
    public sealed class ExportOrdersRequest : Request
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public long OrganizationUnitId { get; set; }
    }
}
