using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability.Resolvers;

namespace DoubleGis.Erm.Platform.Core.Metadata
{
    public sealed class OperationAcceptabilityRegistrar : IOperationAcceptabilityRegistrar
    {
        private readonly IReadOnlyDictionary<int, OperationApplicability> _initialOperationApplicability;

        public OperationAcceptabilityRegistrar(IOperationsRegistry operationsRegistry, IOperationApplicabilityByMetadataResolver operationApplicabilityByMetadataResolver)
        {
            _initialOperationApplicability =
                operationApplicabilityByMetadataResolver
                    .ResolveOperationsApplicability(
                        operationsRegistry.EntitySpecificOperations, 
                        operationsRegistry.NotCoupledOperations, 
                        operationsRegistry.Operations2IdentitiesMap)
                    .ApplyOverrides();
        }

        public IReadOnlyDictionary<int, OperationApplicability> InitialOperationApplicability
        {
            get
            {
                return _initialOperationApplicability;
            }
        }
    }
}