using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Territories
{
    public class ActiveTerritoryCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (TerritoryViewModel)viewModel;
            if (entityViewModel.IsActive)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                                                                     .Where(x => x.Name != "Activate")
                                                                                     .ToArray();
            }
        }
    }
}