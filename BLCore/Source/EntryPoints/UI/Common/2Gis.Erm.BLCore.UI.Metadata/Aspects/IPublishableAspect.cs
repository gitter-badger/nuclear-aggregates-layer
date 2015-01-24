namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects
{
    public interface IPublishableAspect : IAspect
    {
        bool IsPublished { get; }
    }
}