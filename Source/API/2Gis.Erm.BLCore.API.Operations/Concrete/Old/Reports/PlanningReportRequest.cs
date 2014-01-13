using System;
using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Reports
{
    public sealed class PlanningReportRequest : Request
    {
        public long OrganizationUnitId { get; set; }
        public DateTime PlanningMonth { get; set; }
        public bool IsAdvertisingAgency { get; set; }
    }

    public sealed class PlanningReportResponse : Response
    {
        public Stream OutputStream { get; set; }
        public string ContentType { get; set; }
    }
}
