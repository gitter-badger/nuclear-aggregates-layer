using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

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