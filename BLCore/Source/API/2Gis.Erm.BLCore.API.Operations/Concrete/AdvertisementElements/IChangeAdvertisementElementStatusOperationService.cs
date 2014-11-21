using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    public interface IChangeAdvertisementElementStatusOperationService : IOperation<ChangeAdvertisementElementStatusIdentity>
    {
        void ChangeStatus(long advertisementElementId,
                          AdvertisementElementStatusValue newStatus,
                          IEnumerable<AdvertisementElementDenialReason> denialReasons);
    }
}