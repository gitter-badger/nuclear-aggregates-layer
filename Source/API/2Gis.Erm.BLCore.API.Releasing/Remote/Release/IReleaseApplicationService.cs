using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Release201308)]
    public interface IReleaseApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ReleasingErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Release201308)]
        ReleaseStartingResult Start(int organizationUnitDgppId, TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors);

        [OperationContract]
        [FaultContract(typeof(ReleasingErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Release201308)]
        void Attach(long releaseId, ExternalReleaseProcessingMessage[] messages);

        [OperationContract]
        [FaultContract(typeof(ReleasingErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Release201308)]
        void Succeeded(long releaseId);

        [OperationContract]
        [FaultContract(typeof(ReleasingErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Release201308)]
        void Failed(long releaseId);
    }
}