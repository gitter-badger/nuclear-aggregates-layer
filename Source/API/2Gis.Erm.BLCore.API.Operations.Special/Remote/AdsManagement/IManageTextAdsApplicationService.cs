using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.AdsManagement.ManageTextAds201407)]
    public interface IManageTextAdsApplicationService
    {
        [OperationContract(Name = "UpdatePlainTextRest")]
        [FaultContract(typeof(ManageTextAdsErrorDescription), Namespace = ServiceNamespaces.AdsManagement.ManageTextAds201407)]
        void UpdatePlainText(long adsElementId, string plainText);

        [OperationContract(Name = "UpdateSimpleTextRest")]
        [FaultContract(typeof(ManageTextAdsErrorDescription), Namespace = ServiceNamespaces.AdsManagement.ManageTextAds201407)]
        void UpdateFormattedText(long adsElementId, string formattedText);
    }
}