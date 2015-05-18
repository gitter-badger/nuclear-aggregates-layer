using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Reopen
{   
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Reopen201502)]
    public class ReopenOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ReopenOperationErrorDescription(IEntityType entityName, string message)
        {
            EntityName = entityName.Description;
            Message = message;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}
