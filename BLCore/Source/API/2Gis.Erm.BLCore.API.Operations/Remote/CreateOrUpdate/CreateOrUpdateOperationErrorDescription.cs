using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.CreateOrUpdate201304)]
    public class CreateOrUpdateOperationErrorDescription : IBasicOperationErrorDescription
    {
        public CreateOrUpdateOperationErrorDescription(EntityName entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}
