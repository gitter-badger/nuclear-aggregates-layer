using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate
{
    [DataContract(Namespace =  ServiceNamespaces.BasicOperations.Deactivate201303)]
    public class DeactivateOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DeactivateOperationErrorDescription(IEntityType entityName, string message, long ownerCode)
        {
            EntityName = entityName.Description;
            Message = message;
            OwnerCode = ownerCode;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long OwnerCode { get; private set; }
    }
}