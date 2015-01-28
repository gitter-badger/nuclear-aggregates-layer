using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public interface IMessageExpressionFeature : IUIElementLambdaExpressionFeature
    {
        IStringResourceDescriptor MessageDescriptor { get; }
        MessageType MessageType { get; }
    }
}