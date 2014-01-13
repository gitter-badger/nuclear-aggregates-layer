using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class DealViewModelCustomizationService : IGenericViewModelCustomizationService<Deal>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (DealViewModel)viewModel;

            if (entityViewModel.IsActive)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar.DisableButtons("ReopenDeal");
            }
        }
    }
}