using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public class ChangeEntityClientValidationResult
    {
        [DataMember]
        public IEnumerable<string> Warnings { get; set; }
        [DataMember]
        public IEnumerable<string> Errors { get; set; }
    }

    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public class ChangeEntityClientResult
    {
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public bool CanProceed { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    public interface IChangeEntityClientService : IOperation<ChangeClientIdentity>
    {
        ChangeEntityClientValidationResult Validate(long entityId, long clientId);
        ChangeEntityClientResult Execute(long entityId, long clientId, bool bypassValidation);
    }
}