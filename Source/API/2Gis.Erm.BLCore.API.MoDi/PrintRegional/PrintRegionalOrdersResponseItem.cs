using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.PrintRegional
{
    [DataContract]
    public sealed class PrintRegionalOrdersResponseItem
    {
        [DataMember]
        public long SourceOrganizationUnitId { get; set; }
        [DataMember]
        public long SourceBranchOfficeOrganizationUnitId { get; set; }

        [DataMember]
        public long DestOrganizationUnitId { get; set; }
        [DataMember]
        public long DestBranchOfficeOrganizationUnitId { get; set; }
        [DataMember]
        public string DestOrganizationUnitSyncCode1C { get; set; }

        [DataMember]
        public decimal TotalAmount { get; set; }

        [DataMember]
        public FirmWithOrders[] FirmWithOrders { get; set; }

        [DataMember]
        public FileDescription File { get; set; }
    }
}
