using System;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Storage.Core;
using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    public sealed class UnityTransactedOperationScopeFactory : IOperationScopeFactory
    {
        private readonly IUnityContainer _container;
        private readonly IOperationSecurityRegistryReader _operationSecurityRegistryReader;

        public UnityTransactedOperationScopeFactory(IUnityContainer container, IOperationSecurityRegistryReader operationSecurityRegistryReader)
        {
            _container = container;
            _operationSecurityRegistryReader = operationSecurityRegistryReader;
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity>(params IEntityType[] operationEntities) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor(operationEntities), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>() 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new() 
            where TEntity : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor<TEntity>(), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>() 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new() 
            where TEntity1 : class, IEntity 
            where TEntity2 : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor<TEntity1, TEntity2>(), null);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity>(IOperationScope parentScope, params IEntityType[] operationEntities) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor(operationEntities), parentScope);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity>(IOperationScope parentScope) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new() 
            where TEntity : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor<TEntity>(), parentScope);
        }

        public IOperationScope CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>(IOperationScope parentScope) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new() 
            where TEntity1 : class, IEntity 
            where TEntity2 : class, IEntity
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.SpecificFor<TEntity1, TEntity2>(), parentScope);
        }

        public IOperationScope CreateNonCoupled<TOperationIdentity>() 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.NonCoupled(), null);
        }

        public IOperationScope CreateNonCoupled<TOperationIdentity>(IOperationScope parentScope) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return CreateScope(identity.NonCoupled(), parentScope);
        }

        private IOperationScope CreateScope(StrictOperationIdentity operationIdentity, IOperationScope parentScope)
        {
            var processingContext = _container.Resolve<IProcessingContext>();

            bool isRootScope;
            var scopesStorage = EnsureScopesStorageAttachedToProcessingContext(processingContext, out isRootScope);
            var changesRegistry = EnsurePersistenceChangesRegistryAttachedToProcessingContext(processingContext);

            var operationScopesLifetimeManager = _container.Resolve<IOperationScopeLifetimeManager>(
                new ResolverOverride[]
                    {
                        new DependencyOverride(typeof(IOperationScopeContextsStorage), scopesStorage),
                        new DependencyOverride(typeof(IOperationScopeRegistrar), scopesStorage),
                        new DependencyOverride(typeof(IOperationConsistencyContextsProvider),
                                               new OperationConsistencyContextsProvider(changesRegistry, _operationSecurityRegistryReader))
                    });

            Guid createdScopeId = Guid.NewGuid();

            var createdScope = new TransactedOperationScope(createdScopeId,
                                                            isRootScope,
                                                            operationIdentity,
                                                            scopesStorage,
                                                            operationScopesLifetimeManager);

            IOperationScopeRegistrar scopeRegistrar = scopesStorage;

            if (isRootScope)
            {
                if (parentScope != null)
                {
                    throw new InvalidOperationException("Inconsistent behavior - parent scope specified explicitly, but appropriate operation scopes storage was not found");
                }

                scopeRegistrar.RegisterRoot(createdScope);
            }
            else if (parentScope != null)
            {
                scopeRegistrar.RegisterChild(createdScope, parentScope);
            }
            else
            {
                scopeRegistrar.RegisterAutoResolvedChild(createdScope);
            }

            return createdScope;
        }

        private OperationScopesStorage EnsureScopesStorageAttachedToProcessingContext(IProcessingContext processingContext, out bool isRootScope)
        {
            isRootScope = false;

            OperationScopesStorage scopesStorage = processingContext.GetValue(OperationScopesStorageKey.Instance, false);
            if (scopesStorage == null)
            {
                isRootScope = true;
                scopesStorage = _container.Resolve<OperationScopesStorage>();
                processingContext.SetValue(OperationScopesStorageKey.Instance, scopesStorage);
            }

            return scopesStorage;
        }

        private IPersistenceChangesRegistry EnsurePersistenceChangesRegistryAttachedToProcessingContext(IProcessingContext processingContext)
        {
            IPersistenceChangesRegistry changesRegistry = processingContext.GetValue(PersistenceChangesRegistryKey.Instance, false);
            if (changesRegistry == null)
            {
                changesRegistry = _container.Resolve<PersistenceChangesRegistry>();
                processingContext.SetValue(PersistenceChangesRegistryKey.Instance, changesRegistry);
            }

            return changesRegistry;
        }
    }
}