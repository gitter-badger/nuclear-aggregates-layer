using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopeLifetimeManager : IOperationScopeLifetimeManager
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IOperationLogger _operationLogger;
        private readonly IOperationScopeContextsStorage _operationScopeContextsStorage;
        private readonly IOperationScopeRegistrar _operationScopeRegistrar;
        private readonly IOperationConsistencyContextsProvider _verifierContextsProvider;
        private readonly IOperationConsistencyVerifier _operationConsistencyVerifier;
        private readonly IProcessingContext _processingContext;
        private readonly ITracer _tracer;

        public OperationScopeLifetimeManager(
            IEnvironmentSettings environmentSettings,
            IOperationLogger operationLogger,
            IOperationScopeContextsStorage operationScopeContextsStorage, 
            IOperationScopeRegistrar operationScopeRegistrar,
            IOperationConsistencyContextsProvider verifierContextsProvider,
            IOperationConsistencyVerifier operationConsistencyVerifier,
            IProcessingContext processingContext,
            ITracer tracer)
        {
            _environmentSettings = environmentSettings;
            _operationLogger = operationLogger;
            _operationScopeContextsStorage = operationScopeContextsStorage;
            _operationScopeRegistrar = operationScopeRegistrar;
            _verifierContextsProvider = verifierContextsProvider;
            _operationConsistencyVerifier = operationConsistencyVerifier;
            _processingContext = processingContext;
            _tracer = tracer;
        }

        public void Close(IOperationScope scope)
        { 
            // удаляем регистрацию scope всегда, даже если он не completed, т.е. скорее всего произошел неперехваченный exception 
            // и сработал просто scope dispose => это необходимо т.к. код может использоваться в разных сценариях, например, с использование threadpooling подхода (пример - quartz и его simplethreadpool),
            // a, следовательно, этот же поток со всем мусором в TLS если не сделать явной очистки, может быть переиспользован, т.е. при новом логическом потоке выполняемом, на том же самом физическом потоке из пула,
            // в котором ранее был exception - можем получить доступ к мусору
            _operationScopeRegistrar.Unregister(scope);

            if (scope.IsRoot)
            {
                ClearScopeDataFromProcessingContext();

                if (scope.Completed)
                {
                    LogScopeChanges(_operationScopeContextsStorage.UseCase);
                }
            }
        }

        private void LogScopeChanges(TrackedUseCase useCase)
        {
            var verifierContexts = _verifierContextsProvider.GetContexts(useCase);
            if (!_operationConsistencyVerifier.IsOperationContextConsistent(verifierContexts))
            {
                var msg = string.Format("Operation verifier. Operation context is not consistent. Use case root operation identity: {0}", useCase.RootNode.OperationIdentity);
                _tracer.Error(msg);

                // TODO {all, 07.08.2013}: подумать в каких условиях бросать exception, в каких нет (например, development и test environment - бросаем exception, production - просто логируем)
                if (_environmentSettings.Type == EnvironmentType.Development
                || _environmentSettings.Type == EnvironmentType.Test)
                {
                    throw new NotificationException(msg);
                }
            }

            _operationLogger.Log(useCase);
        }

        private void ClearScopeDataFromProcessingContext()
        {
            // закрылся root scope нужно очистить processingcontext от его следов
            _processingContext.SetValue(OperationScopesStorageKey.Instance, null);
            _processingContext.SetValue(PersistenceChangesRegistryKey.Instance, null);
        }
    }
}