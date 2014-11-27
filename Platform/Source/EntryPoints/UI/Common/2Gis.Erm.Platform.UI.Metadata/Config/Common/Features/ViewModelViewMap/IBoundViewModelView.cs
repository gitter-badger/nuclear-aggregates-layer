using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IBoundViewModelView : IMetadataElementAspect
    {
        IViewModelViewMapping ViewModelViewMapping { get; }
    }
}
