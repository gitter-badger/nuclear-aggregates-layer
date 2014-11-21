using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement
{
    [DataContract(Namespace = ServiceNamespaces.AdsManagement.HandleAdsState201407)]
    public class HandleAdsStateErrorDescription
    {
        public HandleAdsStateErrorDescription(long adsElementId, AdvertisementElementStatusValue statusValue, string message)
        {
            AdsElementId = adsElementId;
            StatusValue = statusValue;
            Message = message;
        }

        [DataMember]
        public long AdsElementId { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public AdvertisementElementStatusValue StatusValue { get; private set; }
    }
}