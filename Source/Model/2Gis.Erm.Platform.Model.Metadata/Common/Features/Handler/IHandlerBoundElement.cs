namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler
{
    public interface IHandlerBoundElement : IConfigElementAspect
    {
        IHandlerFeature Handler { get; }
        bool HasHandler { get; }
    }
}