namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface IPublishableAspect : IAspect
    {
        bool IsPublished { get; }
    }
}