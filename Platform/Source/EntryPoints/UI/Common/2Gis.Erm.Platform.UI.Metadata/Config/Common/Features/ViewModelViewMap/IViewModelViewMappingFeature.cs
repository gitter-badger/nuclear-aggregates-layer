using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IViewModelViewMappingFeature : IMetadataFeature
    {
        IViewModelViewMapping Mapping { get; }
    }
}
