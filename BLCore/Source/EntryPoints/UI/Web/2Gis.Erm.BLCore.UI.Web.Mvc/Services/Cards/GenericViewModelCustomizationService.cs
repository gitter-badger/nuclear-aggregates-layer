using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class GenericViewModelCustomizationService : IViewModelCustomizationService
    {
        private readonly IViewModelCustomizationProvider _viewModelCustomizationProvider;

        public GenericViewModelCustomizationService(IViewModelCustomizationProvider viewModelCustomizationProvider)
        {
            _viewModelCustomizationProvider = viewModelCustomizationProvider;
        }

        public void CustomizeViewModel<TModel, TEntity>(TModel viewModel, ModelStateDictionary modelState)
            where TModel : IEntityViewModelBase
            where TEntity : IEntityKey
        {
            var customizations = _viewModelCustomizationProvider.GetCustomizations<TModel, TEntity>();
            foreach (var customization in customizations)
            {
                customization.Customize(viewModel, modelState);
            }
        }
    }
}