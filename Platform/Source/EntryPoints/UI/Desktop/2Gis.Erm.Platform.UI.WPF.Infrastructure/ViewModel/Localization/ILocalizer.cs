using DoubleGis.Platform.UI.WPF.Infrastructure.Localization;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public interface ILocalizer : IViewModelAspect
    {
        DynamicResourceDictionary Localized { get; }
    }
}
