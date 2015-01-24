namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IAdvertisementElementVerificationAspect : IAdvertisementElementRequiresVerificationAspect, IAdvertisementElementVerificationStateAspect
    {
        bool CanUserChangeStatus { get; }
    }
}
