using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CancelActivity
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.CancelActivity201502)]
    public sealed class CancelActivityResult : IOperationResult
    {
        [DataMember]
        public long EntityId { get;  set; }
        [DataMember]
        public bool Succeeded { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
