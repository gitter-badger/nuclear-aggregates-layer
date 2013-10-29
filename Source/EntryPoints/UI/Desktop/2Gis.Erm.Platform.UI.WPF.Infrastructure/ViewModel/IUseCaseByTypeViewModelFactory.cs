using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public interface IByTypeViewModelFactory
    {
        TViewModel Create<TViewModel>(IUseCase useCase) where TViewModel : class, IViewModel;
        IViewModel Create(IUseCase useCase, Type viewModelType);
    }
}
