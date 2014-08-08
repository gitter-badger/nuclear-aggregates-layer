using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
    public interface IHandleAdsStateApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(HandleAdsStateErrorDescription), Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
        void TransferToDraft(long adsElementId);

        [OperationContract]
        [FaultContract(typeof(HandleAdsStateErrorDescription), Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
        void TransferToReadyForValidation(long adsElementId);

        [OperationContract]
        [FaultContract(typeof(HandleAdsStateErrorDescription), Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
        void TransferToApproved(long adsElementId);
    }
}