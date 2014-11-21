using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    [DataContract]
    public sealed class ValidationResult
    {
        [DataMember]
        public int OrderCount { get; set; }
        [DataMember]
        public OrderValidationMessage[] Messages { get; set; } 
    }
}