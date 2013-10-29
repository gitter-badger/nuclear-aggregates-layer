using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsHost : IConfigElementAspect
    {
        bool HasRelatedItems { get; }
        HierarchyElement[] RelatedItems { get; }
    }
}
