using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IViewModelViewMappingFeature : IConfigFeature
    {
        IViewModelViewMapping Mapping { get; }
    }
}
