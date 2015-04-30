using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationConsistencyContextsProvider : IOperationConsistencyContextsProvider
    {
        private readonly IPersistenceChangesRegistry _persistenceChangesRegistry;
        private readonly IOperationSecurityRegistryReader _operationsSecurityRequirements;

        public OperationConsistencyContextsProvider(
            IPersistenceChangesRegistry persistenceChangesRegistry,
            IOperationSecurityRegistryReader operationsSecurityRequirements)
        {
            _persistenceChangesRegistry = persistenceChangesRegistry;
            _operationsSecurityRequirements = operationsSecurityRequirements;
        }

        public IEnumerable<IVerifierContext> GetContexts(TrackedUseCase useCase)
        {
            var contexts = new IVerifierContext[]
                {
                    new VerifierContext<IPersistenceChangesRegistry>(useCase, _persistenceChangesRegistry),
                    new VerifierContext<IOperationSecurityRegistryReader>(useCase, _operationsSecurityRequirements)
                };
            return contexts;
        }
    }
}