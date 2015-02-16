using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Revert
{   
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Revert201502)]
    public class RevertOperationErrorDescription : IBasicOperationErrorDescription
    {
        public RevertOperationErrorDescription(EntityName entityName, string message)
        {
            this.EntityName = entityName;
            this.Message = message;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}
