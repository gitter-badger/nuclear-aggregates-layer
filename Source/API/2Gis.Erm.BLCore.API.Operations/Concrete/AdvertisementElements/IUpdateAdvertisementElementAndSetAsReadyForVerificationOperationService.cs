using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    public interface IUpdateAdvertisementElementAndSetAsReadyForVerificationOperationService : IOperation<UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity>
    {
        void UpdateAndSetAsReadyForVerification(AdvertisementElementDomainEntityDto advertisementElementDomainEntityDto);
    }
}