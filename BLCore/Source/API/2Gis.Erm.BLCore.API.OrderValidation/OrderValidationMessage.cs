using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    [DataContract]
    public sealed class OrderValidationMessage
    {
        [DataMember(IsRequired = false, Name = "AdditionalInfo")]
        private Dictionary<string, object> _additionalInfo;

        [DataMember(IsRequired = false, Name = "OrderId")]
        private string _orderId;

        [IgnoreDataMember]
        public Dictionary<string, object> AdditionalInfo
        {
            get { return _additionalInfo ?? (_additionalInfo = new Dictionary<string, object>(2)); }
        }

        [DataMember]
        public int RuleCode { get; set; }
        [DataMember]
        public MessageType Type { get; set; }

        [IgnoreDataMember]
        public long OrderId
        {
            get { return string.IsNullOrWhiteSpace(_orderId) ? 0 : long.Parse(_orderId); }
            set { _orderId = value.ToString(); }
        }

        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public string MessageText { get; set; }
    }
}
