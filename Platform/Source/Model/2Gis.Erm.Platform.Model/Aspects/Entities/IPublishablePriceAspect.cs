namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IPublishablePriceAspect : IAspect
    {
        bool PriceIsPublished { get; }
    }
}
