namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IPriceAspect : IAspect
    {
        string PriceName { get; }
        long PriceId { get; }
    }
}