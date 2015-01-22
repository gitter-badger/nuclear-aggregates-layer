using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Deactivate201303)]
    public class DeactivateConfirmation
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string ConfirmationMessage { get; set; }
    }

    public interface IDeactivateEntityService : IOperation<DeactivateIdentity>
    {
        DeactivateConfirmation Deactivate(long entityId, long ownerCode);
    }
}
