using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public interface IDialApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(DialErrorDescription), Namespace = ServiceNamespaces.Dialing.Dial201503)]
        DialResult Dial(string phone);
    }
}
