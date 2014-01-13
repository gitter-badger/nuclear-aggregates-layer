using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Reports
{
    public sealed class LegalPersonPaymentsRequest : Request
    {
        public int OrganizationUnitId { get; set; }
        public DateTime ReportDate { get; set; }
        public int CurrentUser { get; set; }
        public bool ConsiderOnRegistration { get; set; }
        public bool ConsiderOnApproval { get; set; }
    }
}
