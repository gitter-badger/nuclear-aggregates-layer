using System.Collections.Generic;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements
{
    public interface IDenyAdvertisementElementOperationService : IOperation<DenyAdvertisementElementIdentity>, IAdvertisementElementStatusChangingStrategy
    {
        int Deny(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons);
    }
}