using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public class ExportAccountDetailsToServiceBusForBranchRequest : Request
    {
        public DateTime StartPeriodDate { get; set; }
        public DateTime EndPeriodDate { get; set; }
        public long OrganizationUnitId { get; set; }
    }
}