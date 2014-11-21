using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    public interface IDynamicPropertiesViewModel
    {
    }

    public interface IDynamicPropertiesViewModel<TViewModel> : IDynamicPropertiesViewModel
        where TViewModel : class, IViewModel
    {
    }
}