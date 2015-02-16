using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Cancel
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Cancel201502)]
    public class CancelOperationErrorDescription : IBasicOperationErrorDescription
    {
        public CancelOperationErrorDescription(EntityName entityName, string message)
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
