using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public sealed class AssignCommonParameter : ICommonOperationParameter
    {
        public AssignCommonParameter(Guid operationToken)
        {
            OperationToken = operationToken;
        }

        [DataMember]
        public Guid OperationToken { get; private set; }
        [DataMember]
        public EntityName EntityName { get; set; }
        [DataMember]
        public long OwnerCode { get; set; }
        [DataMember]
        public bool BypassValidation { get; set; }
        [DataMember]
        public bool IsPartialAssign { get; set; }
    }
}