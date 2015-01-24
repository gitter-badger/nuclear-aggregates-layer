namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IAdvertisementElementRequiresVerificationAspect : IAspect
    {
        bool NeedsValidation { get; }
    }
}
