using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
    public class DisqualifyOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DisqualifyOperationErrorDescription(IEntityType entityName, string message, bool bypassValidation)
        {
            EntityName = entityName.Description;
            Message = message;
            BypassValidation = bypassValidation;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public bool BypassValidation { get; set; }
    }
}