using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class PricePositionViewModelCustomizationService : IGenericViewModelCustomizationService<PricePosition>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (PricePositionViewModel)viewModel;
            if (entityViewModel.IsDeleted)
            {
                entityViewModel.SetInfo(BLResources.CantEditDeactivatedPricePosition);
                entityViewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}