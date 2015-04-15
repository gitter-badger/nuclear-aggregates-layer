namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IAdvertisementElementRequiresVerificationAspect : IAspect
    {
        bool NeedsValidation { get; }
    }
}
