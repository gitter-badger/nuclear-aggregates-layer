using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public interface IUiElementExpressionFeature : IUiElementFeature
    {
        bool TryExecute(IViewModelAbstract viewModel, out bool result);
    }
}
