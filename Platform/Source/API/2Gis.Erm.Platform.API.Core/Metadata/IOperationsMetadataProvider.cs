using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IOperationsMetadataProvider
    {
        IOperationMetadata GetOperationMetadata(IOperationIdentity operationIdentity, params EntityName[] operationProcessingEntities);
            
        TOperationMetadata GetOperationMetadata<TOperationMetadata, TOperationIdentity>(params EntityName[] operationProcessingEntities)
            where TOperationMetadata : class, IOperationMetadata<TOperationIdentity>
            where TOperationIdentity : IOperationIdentity, new();

        bool IsSupported<TOperationIdentity>(params EntityName[] operationProcessingEntities) where TOperationIdentity : IOperationIdentity, new();

        OperationApplicability[] GetApplicableOperations();
        OperationApplicability[] GetApplicableOperationsForCallingUser();
        OperationApplicability[] GetApplicableOperationsForContext(EntityName[] entityNames, long[] entityIds);
    }
}