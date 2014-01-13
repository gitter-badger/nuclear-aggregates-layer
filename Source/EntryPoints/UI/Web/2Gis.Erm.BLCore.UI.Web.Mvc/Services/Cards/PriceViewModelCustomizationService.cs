using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class PriceViewModelCustomizationService : IGenericViewModelCustomizationService<Price>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (PriceViewModel)viewModel;
            if (!entityViewModel.IsNew)
            {
                if (entityViewModel.IsDeleted)
                {
                    entityViewModel.SetInfo(BLResources.CantEditPriceWhenDeactivated);
                    entityViewModel.ViewConfig.ReadOnly = true;
                }

                if (entityViewModel.IsPublished)
                {
                    entityViewModel.SetInfo(BLResources.CantEditPriceWhenPublished);
                    entityViewModel.ViewConfig.ReadOnly = true;
                }
            }

            var publishButton = entityViewModel.ViewConfig.FindCardToolbarItem("PublishPrice");

            if (!publishButton.Disabled && entityViewModel.IsPublished)
            {
                publishButton.Disabled = true;
            }

            var unpublishButton = entityViewModel.ViewConfig.FindCardToolbarItem("UnpublishPrice");

            if (!unpublishButton.Disabled && !entityViewModel.IsPublished)
            {
                unpublishButton.Disabled = true;
            }
        }
    }
}