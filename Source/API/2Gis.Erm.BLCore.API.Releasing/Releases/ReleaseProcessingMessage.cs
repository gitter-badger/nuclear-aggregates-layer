using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Release201308)]
    public sealed class ReleaseProcessingMessage
    {
        [DataMember]
        public long? OrderId { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public bool IsBlocking { get; set; }

        [DataMember]
        public string RuleCode { get; set; }

        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("OrderId: {0}, IsBlocking: {1}, RuleCode: [{2}], Message: {3}", OrderId, IsBlocking, RuleCode, Message);
        }
    }
}