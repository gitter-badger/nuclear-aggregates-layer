using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class TerritoryViewModelCustomizationService : IGenericViewModelCustomizationService<Territory>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (TerritoryViewModel)viewModel;
            if (entityViewModel.IsActive && !entityViewModel.IsDeleted)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                                                                     .Where(x => x.Name != "Activate")
                                                                                     .ToArray();
            }
        }
    }
}