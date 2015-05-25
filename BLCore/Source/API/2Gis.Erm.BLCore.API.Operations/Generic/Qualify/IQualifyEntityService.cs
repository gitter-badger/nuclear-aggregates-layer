using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public class QualifyResult
    {
        [DataMember]
        public long? RelatedEntityId { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public long EntityId { get; set; }
    }

    public interface IQualifyEntityService : IOperation<QualifyIdentity>
    {
        QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId);
    }
}