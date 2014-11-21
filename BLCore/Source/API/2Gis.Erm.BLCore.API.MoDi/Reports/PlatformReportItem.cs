using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [DataContract]
    public class PlatformReportItem
    {
        [DataMember]
        public string FirmName { get; set; }

        [DataMember]
        public string DestOrganizationUnitName { get; set; }

        [DataMember]
        public string SourceOrganizationUnitName { get; set; }

        [DataMember]
        public decimal PayablePrice { get; set; }

        [DataMember]
        public string OrderType { get; set; }

        [DataMember]
        public decimal? DiscountPercent { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string OwnerName { get; set; }

        [DataMember]
        public string LegalPersonName { get; set; }

        [DataMember]
        public string BranchOfficeOrganizationUnitName { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime BeginDistributionDate { get; set; }

        [DataMember]
        public DateTime EndDistributionDatePlan { get; set; }

        [DataMember]
        public short ReleaseCountPlan { get; set; }

        [DataMember]
        public string OrderState { get; set; }

        [DataMember]
        public DateTime EndDistributionDateFact { get; set; }

        [DataMember]
        public short ReleaseCountFact { get; set; }

        [DataMember]
        public decimal PayablePlan { get; set; }

        [DataMember]
        public string PlatformName { get; set; }

        [DataMember]
        public decimal? Rate { get; set; }
    }
}