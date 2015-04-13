using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        IResourceDescriptor NameDescriptor { get;  }
        ITitleDescriptor TitleDescriptor { get; }
        UIElementMetadata[] RelatedItems { get; }
    }
}
