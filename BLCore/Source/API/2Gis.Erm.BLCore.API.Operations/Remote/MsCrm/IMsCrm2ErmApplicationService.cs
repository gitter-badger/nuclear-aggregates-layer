using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.MsCrm
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
    public interface IMsCrm2ErmApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(MsCrm2ErmErrorDescription), Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
        void UpdateAfterSaleActivity(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType afterSaleServiceType);
        [OperationContract]
        [FaultContract(typeof(MsCrm2ErmErrorDescription), Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
        void ReplicateDealStage(Guid dealReplicationCode, MsCrmDealStage dealStage, string userDomainName);
    }
}
