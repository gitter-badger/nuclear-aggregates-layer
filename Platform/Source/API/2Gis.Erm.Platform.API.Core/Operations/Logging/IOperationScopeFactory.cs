using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationScopeFactory
    {
        IOperationScope CreateSpecificFor<TOperationIdentity>(params IEntityType[] operationEntities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new();

        IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : class, IEntity;

        IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity;

        IOperationScope CreateSpecificFor<TOperationIdentity>(IOperationScope parentScope, params IEntityType[] operationEntities) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new();

        IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : class, IEntity;

        IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity;

        IOperationScope CreateNonCoupled<TOperationIdentity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new();

        IOperationScope CreateNonCoupled<TOperationIdentity>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new();
    }
}
