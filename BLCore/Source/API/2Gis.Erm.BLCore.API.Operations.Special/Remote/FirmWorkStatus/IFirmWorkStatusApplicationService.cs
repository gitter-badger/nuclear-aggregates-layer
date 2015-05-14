using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.FirmWorkStatus;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.FirmWorkStatus
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.FinancialOperations.FirmWorkStatus201505)]
    public interface IFirmWorkStatusApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(FirmWorkStatusErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FirmWorkStatus201505)]
        Special.FirmWorkStatus.FirmWorkStatus GetFirmWorkStatus(long firmId);
    }
}