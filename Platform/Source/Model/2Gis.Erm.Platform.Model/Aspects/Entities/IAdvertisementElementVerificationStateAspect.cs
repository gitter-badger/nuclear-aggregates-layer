using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IAdvertisementElementVerificationStateAspect : IAspect
    {
        AdvertisementElementStatusValue Status { get; }
    }
}
