using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public interface IValidatableViewModel
    {
    }

    public interface IValidatableViewModel<TViewModel> : IValidatableViewModel
        where TViewModel : class, IViewModel
    {
    }
}
