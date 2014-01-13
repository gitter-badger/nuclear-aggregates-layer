using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class CategoryViewModelCustomizationService : IGenericViewModelCustomizationService<Category>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            viewModel.ViewConfig.ReadOnly = true;
        }
    }
}