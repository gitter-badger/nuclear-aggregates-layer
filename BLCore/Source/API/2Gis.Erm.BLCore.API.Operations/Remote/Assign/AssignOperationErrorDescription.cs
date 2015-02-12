using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Assign
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public class AssignOperationErrorDescription : IBasicOperationErrorDescription
    {
        public AssignOperationErrorDescription(IEntityType entityName, string message, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            EntityName = entityName.Description;
            Message = message;
            OwnerCode = ownerCode;
            BypassValidation = bypassValidation;
            IsPartialAssign = isPartialAssign;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long OwnerCode { get; private set; }
        [DataMember]
        public bool BypassValidation { get; private set; }
        [DataMember]
        public bool IsPartialAssign { get; private set; }
    }
}