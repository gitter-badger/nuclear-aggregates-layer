using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify
{
    public class QualifyResult
    {
        public long? RelatedEntityId { get; set; }
    }

    public interface IQualifyEntityService : IOperation<QualifyIdentity>
    {
        QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId);
    }
}