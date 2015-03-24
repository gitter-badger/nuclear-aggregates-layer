using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Reopen
{   
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Reopen201502)]
    public class ReopenOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ReopenOperationErrorDescription(EntityName entityName, string message)
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
