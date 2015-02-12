using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Activate
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Activate201303)]
    public class ActivateOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ActivateOperationErrorDescription(IEntityType entityName, string message)
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