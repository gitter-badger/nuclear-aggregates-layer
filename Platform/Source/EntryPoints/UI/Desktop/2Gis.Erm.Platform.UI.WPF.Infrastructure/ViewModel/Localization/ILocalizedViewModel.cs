using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Localization;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public interface ILocalizedViewModel
    {
        DynamicResourceDictionary Localizer { get; }
    }

    public interface ILocalizedViewModel<TViewModel> : ILocalizedViewModel
        where TViewModel : class, IViewModel
    {
    }
}
