using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class DeniedPositionViewModelCustomizationService : IGenericViewModelCustomizationService<DeniedPosition>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (DeniedPositionViewModel)viewModel;
            if (entityViewModel.IsNew && entityViewModel.IsPricePublished)
            {
                entityViewModel.SetInfo(BLResources.CantAddDeniedPositionWhenPriceIsPublished);
                entityViewModel.ViewConfig.ReadOnly = true;
                return;
            }

            if (entityViewModel.IsDeleted)
            {
                entityViewModel.SetInfo(BLResources.CantEditDeactivatedDeniedPosition);
                entityViewModel.ViewConfig.ReadOnly = true;
                return;
            }

            if (entityViewModel.IsPricePublished)
            {
                entityViewModel.SetInfo(BLResources.CantEditDeniedPositionWhenPriceIsPublished);
                entityViewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}