using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
    public class DisqualifyOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DisqualifyOperationErrorDescription(EntityName entityName, string message, bool bypassValidation)
        {
            EntityName = entityName;
            Message = message;
            BypassValidation = bypassValidation;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public bool BypassValidation { get; set; }
    }
}