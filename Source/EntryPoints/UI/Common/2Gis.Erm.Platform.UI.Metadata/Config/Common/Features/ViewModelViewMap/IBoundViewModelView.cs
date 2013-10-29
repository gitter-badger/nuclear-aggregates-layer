using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IBoundViewModelView : IConfigElementAspect
    {
        IViewModelViewMapping ViewModelViewMapping { get; }
    }
}
