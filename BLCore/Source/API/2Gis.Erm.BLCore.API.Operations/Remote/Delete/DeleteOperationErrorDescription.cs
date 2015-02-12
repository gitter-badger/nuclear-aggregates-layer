using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Delete
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
    public class DeleteOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DeleteOperationErrorDescription(IEntityType entityName, string message)
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