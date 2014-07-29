using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public sealed class AssignEntityParameter : IOperationParameter
    {
        [DataMember]
        public long EntityId { get; set; }
    }
}