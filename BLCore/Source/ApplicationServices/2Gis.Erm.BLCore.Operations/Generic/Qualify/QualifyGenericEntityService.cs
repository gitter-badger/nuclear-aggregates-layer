using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Qualify
{
    public class QualifyGenericEntityService<TEntity> : IQualifyGenericEntityService<TEntity> 
        where TEntity : class, IEntityKey
    {
        public QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId)
        {
            throw new NotSupportedException("Qualify operation is supported by Firm and Client only");
        }
    }
}