using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.API.Operations.Special.Concrete.Old.Reports
{
    // FIXME {v.lapeev, 27.02.2014}: Создать сборку 2Gis.Erm.BL.API.Operations.Special и перетащить этот реквест туда
    public sealed class LegalPersonPaymentsRequest : Request
    {
        public int OrganizationUnitId { get; set; }
        public DateTime ReportDate { get; set; }
        public int CurrentUser { get; set; }
        public bool ConsiderOnRegistration { get; set; }
        public bool ConsiderOnApproval { get; set; }
    }
}
