using System;
using System.ServiceModel;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [MessageContract(IsWrapped = true)]
    public sealed class PlatformReportRequest
    {
        [MessageBodyMember(Order = 0)]
        public int Type { get; set; }

        [MessageBodyMember(Order = 1)]
        public long OrganizationUnitId { get; set; }

        [MessageBodyMember(Order = 2)]
        public DateTime StartDate { get; set; }

        [MessageBodyMember(Order = 3)]
        public bool SelectOrdersOnRegistration { get; set; }

        [MessageBodyMember(Order = 4)]
        public bool SelectOrdersOnApproval { get; set; }

        [MessageBodyMember(Order = 5)]
        public bool GetDataForAllNetwork { get; set; }
    }
}