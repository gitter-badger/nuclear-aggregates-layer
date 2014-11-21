using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate
{
    [DataContract(Namespace =  ServiceNamespaces.BasicOperations.Deactivate201303)]
    public class DeactivateOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DeactivateOperationErrorDescription(EntityName entityName, string message, long ownerCode)
        {
            EntityName = entityName;
            Message = message;
            OwnerCode = ownerCode;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long OwnerCode { get; private set; }
    }
}