using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public interface ITitledViewModel
    {
        string Title { get; }
    }

    public interface ITitledViewModel<TViewModel> : ITitledViewModel
        where TViewModel : class, IViewModel
    {
    }
}
