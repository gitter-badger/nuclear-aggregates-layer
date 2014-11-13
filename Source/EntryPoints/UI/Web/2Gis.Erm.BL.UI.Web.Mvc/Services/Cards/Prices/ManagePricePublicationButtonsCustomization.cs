using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices
{
    public class ManagePricePublicationButtonsCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var priceViewModel = (PriceViewModel)viewModel;

            var publishButton = priceViewModel.ViewConfig.FindCardToolbarItem("PublishPrice");
            var unpublishButton = priceViewModel.ViewConfig.FindCardToolbarItem("UnpublishPrice");

            if (priceViewModel.IsPublished)
            {
                publishButton.Disabled = true;
            }
            else
            {
                unpublishButton.Disabled = true;
            }
        }
    }
}