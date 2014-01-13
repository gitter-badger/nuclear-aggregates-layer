using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    [DataContract]
    public sealed class ValidateOrdersResult
    {
        [DataMember]
        public int OrderCount { get; set; }
        [DataMember]
        public OrderValidationMessage[] Messages { get; set; } 
    }
}