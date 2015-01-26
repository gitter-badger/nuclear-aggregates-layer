using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public interface IUIElementExpressionFeature : IUIElementFeature
    {
        bool TryExecute(IAspect aspect, out bool result);
    }
}
