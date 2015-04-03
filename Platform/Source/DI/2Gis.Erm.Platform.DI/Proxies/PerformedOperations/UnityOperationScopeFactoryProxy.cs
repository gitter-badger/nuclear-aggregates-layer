using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.DI.Proxies.PerformedOperations
{
    public sealed class UnityOperationScopeFactoryProxy : UnityContainerScopeProxy<IOperationScopeFactory>, IOperationScopeDisposableFactory
    {
        public UnityOperationScopeFactoryProxy(IUnityContainer unityContainer, IOperationScopeFactory proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity>(params IEntityType[] operationEntities)
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity>(operationEntities);
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity, TEntity>()
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity, TEntity>();
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>()
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>();
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity>(IOperationScope parentScope, params IEntityType[] operationEntities)
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity>(parentScope, operationEntities);
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity, TEntity>(IOperationScope parentScope)
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity, TEntity>(parentScope);
        }

        IOperationScope IOperationScopeFactory.CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>(IOperationScope parentScope)
        {
            return ProxiedInstance.CreateSpecificFor<TOperationIdentity, TEntity1, TEntity2>(parentScope);
        }

        IOperationScope IOperationScopeFactory.CreateNonCoupled<TOperationIdentity>()
        {
            return ProxiedInstance.CreateNonCoupled<TOperationIdentity>();
        }

        IOperationScope IOperationScopeFactory.CreateNonCoupled<TOperationIdentity>(IOperationScope parentScope)
        {
            return ProxiedInstance.CreateNonCoupled<TOperationIdentity>(parentScope);
        }
    }
}
