using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        IResourceDescriptor NameDescriptor { get;  }
        ITitleDescriptor TitleDescriptor { get; }
        UIElementMetadata[] RelatedItems { get; }
    }
}
