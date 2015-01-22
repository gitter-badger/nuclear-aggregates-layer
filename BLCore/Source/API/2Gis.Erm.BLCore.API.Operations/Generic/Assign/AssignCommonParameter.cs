using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities;

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
        public IEntityType EntityName { get; set; }
        [DataMember]
        public long OwnerCode { get; set; }
        [DataMember]
        public bool BypassValidation { get; set; }
        [DataMember]
        public bool IsPartialAssign { get; set; }
    }
}