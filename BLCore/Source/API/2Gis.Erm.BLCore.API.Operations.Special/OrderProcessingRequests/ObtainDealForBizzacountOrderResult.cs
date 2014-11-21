using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    [DataContract]
    public sealed class ObtainDealForBizzacountOrderResult
    {
        [DataMember]
        public long DealId { get; set; }

        [DataMember]
        public string DealName { get; set; }

        [DataMember]
        public long DealOwnerCode { get; set; }
    }
}
