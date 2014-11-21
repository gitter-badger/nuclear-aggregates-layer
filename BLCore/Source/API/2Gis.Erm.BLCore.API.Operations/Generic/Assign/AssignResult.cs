using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public sealed class AssignResult : IOperationResult
    {
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public long? OwnerCode { get; set; }
        [DataMember]
        public bool CanProceed { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool Succeeded { get; set; }
    }
}