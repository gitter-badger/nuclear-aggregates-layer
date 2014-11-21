using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    /// <summary>
    /// Маркерный интерфейс для реализации динамического валидатора - динамический он, т.к. предназначен для валидации динамической viewmodel
    /// </summary>
    public interface IDynamicViewModelValidator
    {
    }

    public interface IDynamicViewModelValidator<TViewModel> : IDynamicViewModelValidator
        where TViewModel : class, IDynamicViewModel
    {
    }
}