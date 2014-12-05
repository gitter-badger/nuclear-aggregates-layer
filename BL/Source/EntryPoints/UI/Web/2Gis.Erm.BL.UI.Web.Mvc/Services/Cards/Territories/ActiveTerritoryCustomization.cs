using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Territories
{
    public sealed class ActiveTerritoryCustomization : IViewModelCustomization<TerritoryViewModel>
    {
        public void Customize(TerritoryViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsActive)
            {
                viewModel.ViewConfig.CardSettings.CardToolbar = viewModel.ViewConfig.CardSettings.CardToolbar
                                                                                     .Where(x => x.Name != "Activate")
                                                                                     .ToArray();
            }
        }
    }
}