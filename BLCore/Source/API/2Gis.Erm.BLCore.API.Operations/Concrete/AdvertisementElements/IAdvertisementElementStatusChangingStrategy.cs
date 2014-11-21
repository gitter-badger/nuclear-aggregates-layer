using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    public interface IAdvertisementElementStatusChangingStrategy
    {
        void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons);
        void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons);
    }
}
