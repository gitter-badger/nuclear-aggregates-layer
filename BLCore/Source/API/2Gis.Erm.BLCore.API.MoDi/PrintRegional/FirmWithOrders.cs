using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.PrintRegional
{
    [DataContract]
    public class FirmWithOrders
    {
        [DataMember]
        public long FirmId { get; set; }
        [DataMember]
        public long[] OrderIds { get; set; } 
    }
}