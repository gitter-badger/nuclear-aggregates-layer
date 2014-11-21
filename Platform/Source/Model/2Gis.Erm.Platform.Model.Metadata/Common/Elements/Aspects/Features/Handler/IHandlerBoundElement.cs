namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler
{
    public interface IHandlerBoundElement : IMetadataElementAspect
    {
        IHandlerFeature Handler { get; }
        bool HasHandler { get; }
    }
}