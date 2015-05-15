using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public class ChangeClientOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ChangeClientOperationErrorDescription(IEntityType entityName, string message, long entityId, long clientId, bool? bypassValidation)
        {
            EntityName = entityName.Description;
            Message = message;
            EntityId = entityId;
            ClientId = clientId;
            BypassValidation = bypassValidation;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long EntityId { get; private set; }
        [DataMember]
        public long ClientId { get; private set; }
        [DataMember]
        public bool? BypassValidation { get; private set; }
    }
}