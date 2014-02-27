using System;
using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.API.Operations.Special.Concrete.Old.Reports
{
    // FIXME {v.lapeev, 27.02.2014}: Создать сборку 2Gis.Erm.BL.API.Operations.Special и перетащить этот реквест туда
    public sealed class PlanningReportRequest : Request
    {
        public long OrganizationUnitId { get; set; }
        public DateTime PlanningMonth { get; set; }
        public bool IsAdvertisingAgency { get; set; }
    }

    // FIXME {v.lapeev, 27.02.2014}: Создать сборку 2Gis.Erm.BL.API.Operations.Special и перетащить этот респонс туда
    public sealed class PlanningReportResponse : Response
    {
        public Stream OutputStream { get; set; }
        public string ContentType { get; set; }
    }
}
