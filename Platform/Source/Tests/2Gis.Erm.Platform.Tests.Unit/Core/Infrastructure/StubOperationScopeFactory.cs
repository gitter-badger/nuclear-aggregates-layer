using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Tests.Unit.Core.Infrastructure
{
    public sealed class StubOperationScopeFactory : IOperationScopeFactory
    {
        public IOperationScope CreateSpecificFor<TOperationIdentity>(params IEntityType[] operationEntities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor(operationEntities), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor<TEntity>(), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor<TEntity1, TEntity2>(), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity>(IOperationScope parentScope, params IEntityType[] operationEntities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor(operationEntities), parentScope);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor<TEntity>(), parentScope);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.SpecificFor<TEntity1, TEntity2>(), parentScope);
        }

        public IOperationScope CreateNonCoupled<TOperationIdentity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.NonCoupled(), null);
        }

        public IOperationScope CreateNonCoupled<TOperationIdentity>(IOperationScope parentScope)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateStubOperationScope(identity.NonCoupled(), parentScope);
        }

        private static NullOperationScope CreateStubOperationScope(StrictOperationIdentity operationIdentity, IOperationScope parentScope)
        {
            return new NullOperationScope(parentScope != null, operationIdentity);
        }
    }
}