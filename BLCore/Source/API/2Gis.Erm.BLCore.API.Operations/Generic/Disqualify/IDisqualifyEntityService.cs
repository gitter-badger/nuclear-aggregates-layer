using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
    public class DisqualifyResult
    {
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public bool CanProceed { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    public interface IDisqualifyEntityService : IOperation<DisqualifyIdentity>
    {
        DisqualifyResult Disqualify(long entityId, bool bypassValidation); 
    }
}