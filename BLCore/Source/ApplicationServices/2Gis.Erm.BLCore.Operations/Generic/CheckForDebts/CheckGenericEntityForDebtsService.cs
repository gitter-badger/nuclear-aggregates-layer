using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckGenericEntityForDebtsService<TEntity> : ICheckGenericEntityForDebtsService<TEntity> 
        where TEntity : class, IEntityKey
    {
        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            throw new NotSupportedException("Check for debts operation is supported by Client, LegalPerson and Account only");
        }
    }
}