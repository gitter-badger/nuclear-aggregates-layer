namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IDeletablePriceAspect : IAspect
    {
        bool PriceIsDeleted { get; }
    }
}
