using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy
{
    [DataContract]
    public sealed class CopyOrderResult 
    {
        [DataMember]
        public long OrderId { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
    }
}
