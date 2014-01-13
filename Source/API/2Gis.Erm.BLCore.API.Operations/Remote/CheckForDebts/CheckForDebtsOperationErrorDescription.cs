using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts
{
    [DataContract(Namespace =  ServiceNamespaces.BasicOperations.CheckForDebts201303)]
    public class CheckForDebtsOperationErrorDescription : IBasicOperationErrorDescription
    {
        public CheckForDebtsOperationErrorDescription(EntityName entityName, string message)
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