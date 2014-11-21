using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityByTypeViewModelFactory : IByTypeViewModelFactory
    {
        public TViewModel Create<TViewModel>(IUseCase useCase) 
            where TViewModel : class, IViewModel
        {
            return (TViewModel)Create(useCase, typeof(TViewModel));
        }

        public IViewModel Create(IUseCase useCase, Type viewModelType)
        {
            var container = useCase.ResolveFactoryContext();
            if (!viewModelType.IsViewModel())
            {
                throw new InvalidOperationException("Can't create view model of type " + viewModelType + ". Valid view model must implement interface " + MVVMIndicators.ViewModel);
            }

            return (IViewModel)container.Resolve(viewModelType);
        }
    }
}