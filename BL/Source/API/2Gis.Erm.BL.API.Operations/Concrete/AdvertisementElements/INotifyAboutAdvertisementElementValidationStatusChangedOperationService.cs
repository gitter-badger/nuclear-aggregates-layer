using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements
{
    public interface INotifyAboutAdvertisementElementRejectionOperationService : IOperation<NotifyAboutAdvertisementElementRejectionIdentity>
    {
        void Notify(long advertisementElementId);
    }
}