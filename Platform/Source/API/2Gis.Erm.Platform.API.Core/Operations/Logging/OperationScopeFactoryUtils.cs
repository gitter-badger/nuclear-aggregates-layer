using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public static class OperationScopeFactoryUtils
    {
        public static IOperationScope CreateOrUpdateOperationFor<TEntity>(this IOperationScopeFactory factory, TEntity entity)
            where TEntity : class, IEntityKey, IEntity
        {
            return entity.IsNew()
                       ? factory.CreateSpecificFor<CreateIdentity, TEntity>()
                       : factory.CreateSpecificFor<UpdateIdentity, TEntity>();
        }
    }
}
