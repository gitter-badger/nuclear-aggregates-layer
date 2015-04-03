using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IBoundViewModelView : IMetadataElementAspect
    {
        IViewModelViewMapping ViewModelViewMapping { get; }
    }
}
