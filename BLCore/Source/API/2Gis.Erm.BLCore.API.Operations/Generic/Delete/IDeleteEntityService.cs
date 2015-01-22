using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Delete
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
    public class DeleteConfirmation
    {
        [DataMember]
        public string ConfirmationMessage { get; set; }
    }

    public class DeleteConfirmationInfo
    {
        public string EntityCode { get; set; }
        public bool IsDeleteAllowed { get; set; }
        public string DeleteConfirmation { get; set; }
        public string DeleteDisallowedReason { get; set; }
    }

    public interface IDeleteEntityService : IOperation<DeleteIdentity>
    {
        DeleteConfirmation Delete(long entityId);
        DeleteConfirmationInfo GetConfirmation(long entityId);
    }
}
