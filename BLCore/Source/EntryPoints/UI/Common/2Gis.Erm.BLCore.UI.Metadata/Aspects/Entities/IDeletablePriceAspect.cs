namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IDeletablePriceAspect : IAspect
    {
        bool PriceIsDeleted { get; }
    }
}
