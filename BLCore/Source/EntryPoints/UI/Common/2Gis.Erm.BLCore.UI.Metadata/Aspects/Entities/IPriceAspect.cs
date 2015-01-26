namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IPriceAspect : IAspect
    {
        string PriceName { get; }
        long PriceId { get; }
    }
}