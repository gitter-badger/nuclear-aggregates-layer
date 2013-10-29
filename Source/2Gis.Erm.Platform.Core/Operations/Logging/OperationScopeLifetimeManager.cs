using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopeLifetimeManager : IOperationScopeLifetimeManager
    {
        private readonly IAppSettings _appSettings;
        private readonly IOperationLogger _operationLogger;
        private readonly IOperationScopeContextsStorage _operationScopeContextsStorage;
        private readonly IOperationScopeRegistrar _operationScopeRegistrar;
        private readonly IOperationConsistencyContextsProvider _verifierContextsProvider;
        private readonly IOperationConsistencyVerifier _operationConsistencyVerifier;
        private readonly IProcessingContext _processingContext;
        private readonly ICommonLog _logger;

        public OperationScopeLifetimeManager(
            IAppSettings appSettings,
            IOperationLogger operationLogger,
            IOperationScopeContextsStorage operationScopeContextsStorage, 
            IOperationScopeRegistrar operationScopeRegistrar,
            IOperationConsistencyContextsProvider verifierContextsProvider,
            IOperationConsistencyVerifier operationConsistencyVerifier,
            IProcessingContext processingContext,
            ICommonLog logger)
        {
            _appSettings = appSettings;
            _operationLogger = operationLogger;
            _operationScopeContextsStorage = operationScopeContextsStorage;
            _operationScopeRegistrar = operationScopeRegistrar;
            _verifierContextsProvider = verifierContextsProvider;
            _operationConsistencyVerifier = operationConsistencyVerifier;
            _processingContext = processingContext;
            _logger = logger;
        }

        public void Close(IOperationScope scope)
        { 
            // удаляем регистрацию scope всегда, даже если он не comleted, т.е. скорее всего произошел неперехваченный exception 
            // и сработал просто scope dispose => это необходимо т.к. код может использоваться в разных сценариях, например, с использование threadpooling подхода (пример - quartz и его simplethreadpool),
            // a, следовательно, этот же поток со всем мусором в TLS если не сделать явной очистки, может быть переиспользован, т.е. при новом логическом потоке выполняемом, на том же самом физическом потоке из пула,
            // в котором ранее был exception - можем получить доступ к мусору
            _operationScopeRegistrar.Unregister(scope);

            if (scope.IsRoot)
            {
                ClearScopeDataFromProcessingContext();

                if (scope.Completed)
                {
                    LogScopeChanges(scope);
                }
            }
        }

        private void LogScopeChanges(IOperationScope scope)
        {        
            var verifierContexts = _verifierContextsProvider.GetContexts(_operationScopeContextsStorage.RootScope);
            if (!_operationConsistencyVerifier.IsOperationContextConsistent(verifierContexts))
            {
                var msg = string.Format("Operation verifier. Operation context is not consistent. Root operation identity: {0}", scope.OperationIdentity);
                _logger.ErrorEx(msg);

                // TODO {all, 07.08.2013}: подумать в каких условиях бросать exception, в каких нет (например, development и test environment - бросаем exception, production - просто логируем)
                if (_appSettings.TargetEnvironment == AppTargetEnvironment.Development
                || _appSettings.TargetEnvironment == AppTargetEnvironment.Test)
                {
                    throw new NotificationException(msg);
                }
            }

            _operationLogger.Log(_operationScopeContextsStorage.RootScope);
        }

        private void ClearScopeDataFromProcessingContext()
        {
            // закрылся root scope нужно очистить processingcontext от его следов
            _processingContext.SetValue(OperationScopesStorageKey.Instance, null);
            _processingContext.SetValue(PersistenceChangesRegistryKey.Instance, null);
        }
    }
}