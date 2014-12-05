using System.Collections.Generic;
using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class GenericViewModelCustomizationService<TModel> : IViewModelCustomizationService<TModel>
        where TModel : IEntityViewModelBase
    {
        private readonly IEnumerable<IViewModelCustomization<TModel>> _customizations;

        public GenericViewModelCustomizationService(IEnumerable<IViewModelCustomization<TModel>> customizations)
        {
            _customizations = customizations;
        }

        public void CustomizeViewModel(TModel viewModel, ModelStateDictionary modelState)
        {
            foreach (var customization in _customizations)
            {
                customization.Customize(viewModel, modelState);
            }
        }
    }
}