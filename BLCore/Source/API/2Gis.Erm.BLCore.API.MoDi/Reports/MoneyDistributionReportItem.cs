using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [DataContract]
    public sealed class MoneyDistributionReportItem : PlatformReportItem
    {
        [DataMember]
        public string SaleType { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public decimal DiscountCost { get; set; }
    }
}