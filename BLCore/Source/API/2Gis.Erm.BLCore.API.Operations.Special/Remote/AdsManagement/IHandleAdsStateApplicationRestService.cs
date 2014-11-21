using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
    public interface IHandleAdsStateApplicationRestService
    {
        [OperationContract(Name = "TransferToDraftRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/transfer/to/draft/{adsElementId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(HandleAdsStateErrorDescription))]
        void TransferToDraft(string adsElementId);

        [OperationContract(Name = "TransferToReadyForValidationRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/transfer/to/readyforvalidation/{adsElementId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(HandleAdsStateErrorDescription))]
        void TransferToReadyForValidation(string adsElementId);

        [OperationContract(Name = "TransferToApprovedRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/transfer/to/approved/{adsElementId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(HandleAdsStateErrorDescription))]
        void TransferToApproved(string adsElementId);
    }
}