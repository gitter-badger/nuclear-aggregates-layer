using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public interface IUIElementExpressionFeature : IUIElementFeature
    {
        bool TryExecute(IViewModelAbstract viewModel, out bool result);
    }
}
