using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.MoneyDistibution.WithdrawalInfo.ServiceContract)]
    public interface IWithdrawalInfoApplicationService
    {
        [OperationContract]
        PriceCostInfo[] GetPriceCostsForSubPositions(long parentPositionId, long priceId);
    }
}