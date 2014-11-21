using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [DataContract(Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
    public class ManageTextAdsErrorDescription
    {
        public ManageTextAdsErrorDescription(long adsElementId, string message)
        {
            AdsElementId = adsElementId;
            Message = message;
        }

        [DataMember]
        public long AdsElementId { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}