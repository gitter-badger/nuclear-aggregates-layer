using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IOperationsMetadataProvider
    {
        IOperationMetadata GetOperationMetadata(IOperationIdentity operationIdentity, params IEntityType[] operationProcessingEntities);

        TOperationMetadata GetOperationMetadata<TOperationMetadata, TOperationIdentity>(params IEntityType[] operationProcessingEntities)
            where TOperationMetadata : class, IOperationMetadata<TOperationIdentity>
            where TOperationIdentity : IOperationIdentity, new();

        bool IsSupported<TOperationIdentity>(params IEntityType[] operationProcessingEntities) where TOperationIdentity : IOperationIdentity, new();

        OperationApplicability[] GetApplicableOperations();
        OperationApplicability[] GetApplicableOperationsForCallingUser();
        OperationApplicability[] GetApplicableOperationsForContext(IEntityType[] entityNames, long[] entityIds);
    }
}