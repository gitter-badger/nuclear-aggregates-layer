namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IAdvertisementElementVerificationAspect : IAdvertisementElementRequiresVerificationAspect, IAdvertisementElementVerificationStateAspect
    {
        bool CanUserChangeStatus { get; }
    }
}
