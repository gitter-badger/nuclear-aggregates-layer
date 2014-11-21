using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    public class DisqualifyGenericEntityService<TEntity> : IDisqualifyGenericEntityService<TEntity> where TEntity : class, IEntityKey
    {
        public DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            throw new NotSupportedException("Disqualify operation is supported by Firm and Client only");
        }
    }
}