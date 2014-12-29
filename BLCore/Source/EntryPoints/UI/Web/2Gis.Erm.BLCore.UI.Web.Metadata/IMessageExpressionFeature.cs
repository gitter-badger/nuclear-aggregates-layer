using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Metadata
{
    public interface IMessageExpressionFeature : IUIElementExpressionFeature
    {
        IStringResourceDescriptor MessageDescriptor { get; }
        MessageType MessageType { get; }
    }
}