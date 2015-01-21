using System;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Metadata
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.Metadata.Metadata201307)]
    public interface IMetadataProviderApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Metadata.Metadata201307)]
        OperationApplicability[] GetApplicableOperations();

        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Metadata.Metadata201307)]
        OperationApplicability[] GetApplicableOperationsForCallingUser();

        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Metadata.Metadata201307)]
        OperationApplicability[] GetApplicableOperationsForContext(IEntityType[] entityNames, long[] entityIds);

        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Metadata.Metadata201307)]
        EndpointDescription[] GetAvailableServices();

        [OperationContract]
        [FaultContract(typeof(MetadataOperationErrorDescription), Namespace = ServiceNamespaces.Metadata.Metadata201307)]
        EndpointDescription[] GetCompatibleServices(Version clientVersion);
    }
}
