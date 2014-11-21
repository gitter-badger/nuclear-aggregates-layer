using System;

using DoubleGis.Erm.Platform.DI.Proxies.PerformedOperations;
using DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.PerformedOperationsAnalysis
{
    public sealed class UnityOperationScopeDisposableFactoryAccessor<TOperationScopeFactory> : IOperationScopeDisposableFactoryAccessor
        where TOperationScopeFactory : class, IOperationScopeDisposableFactory
    {
        private readonly Type _concreteFactoryType = typeof(TOperationScopeFactory);
        private readonly IUnityContainer _parentContainer;

        public UnityOperationScopeDisposableFactoryAccessor(IUnityContainer parentContainer)
        {
            _parentContainer = parentContainer;
        }

        public IOperationScopeDisposableFactory Factory
        {
            get
            {
                var targetContainer = _parentContainer.CreateChildContainer();
                return new UnityOperationScopeFactoryProxy(targetContainer, (TOperationScopeFactory)targetContainer.Resolve(_concreteFactoryType));
            }
        }
    }
}
