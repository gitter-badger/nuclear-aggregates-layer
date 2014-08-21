using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.AdsManagement.ManageTextAds201407)]
    public interface IManageTextAdsApplicationRestService
    {
        [OperationContract(Name = "UpdatePlainTextRest")]
        [WebInvoke(Method = "POST", 
                   UriTemplate = "/update/plain/{adsElementId}", 
                   RequestFormat = WebMessageFormat.Json, 
                   BodyStyle  = WebMessageBodyStyle.WrappedRequest, 
                   ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ManageTextAdsErrorDescription))]
        void UpdatePlainText(string adsElementId, string plainText);

        [OperationContract(Name = "UpdateSimpleTextRest")]
        [WebInvoke(Method = "POST", 
                   UriTemplate = "/update/formatted/{adsElementId}",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, 
                   ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ManageTextAdsErrorDescription))]
        void UpdateFormattedText(string adsElementId, string formattedText);
    }
}