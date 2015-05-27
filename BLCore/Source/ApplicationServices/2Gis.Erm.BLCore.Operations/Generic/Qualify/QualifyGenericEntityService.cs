using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using NuClear.Model.Common.Entities.Aspects;

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