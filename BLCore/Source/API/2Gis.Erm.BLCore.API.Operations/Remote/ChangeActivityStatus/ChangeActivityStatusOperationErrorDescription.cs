using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeActivityStatus
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeActivityStatus201302)]
    public class ChangeActivityStatusOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ChangeActivityStatusOperationErrorDescription(EntityName entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        public EntityName EntityName { get; private set; }

        public string Message { get; private set; }
    }
}
