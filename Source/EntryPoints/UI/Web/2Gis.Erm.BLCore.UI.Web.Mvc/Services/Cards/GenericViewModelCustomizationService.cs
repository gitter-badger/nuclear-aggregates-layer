using System.Collections.Generic;
using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class GenericViewModelCustomizationService<TEntity> : IGenericViewModelCustomizationService<TEntity> where TEntity : class, IEntityKey
    {
        private readonly IEnumerable<IViewModelCustomization> _customizations;

        public GenericViewModelCustomizationService(IEnumerable<IViewModelCustomization> customizations)
        {
            _customizations = customizations;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            foreach (var customization in _customizations)
            {
                customization.Customize(viewModel);
            }
        }
    }
}