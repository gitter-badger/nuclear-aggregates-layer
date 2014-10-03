using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class DealViewModelCustomizationService : IGenericViewModelCustomizationService<Deal>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (MultiCultureDealViewModel)viewModel;

            if (entityViewModel.IsActive)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar.DisableButtons("ReopenDeal");
            }
        }
    }
}