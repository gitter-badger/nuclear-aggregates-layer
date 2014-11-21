using System;
using System.ServiceModel;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [MessageContract(IsWrapped = true)]
    public sealed class MoneyDistributionReportRequest
    {
        [MessageBodyMember(Order = 0)]
        public long OrganizationUnitId { get; set; }

        [MessageBodyMember(Order = 1)]
        public DateTime StartDate { get; set; }

        [MessageBodyMember(Order = 2)]
        public bool SelectOrdersOnRegistration { get; set; }

        [MessageBodyMember(Order = 3)]
        public bool SelectOrdersOnApproval { get; set; }

        [MessageBodyMember(Order = 4)]
        public bool GetDataForAllNetwork { get; set; }
    }
}