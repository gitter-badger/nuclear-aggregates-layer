using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices
{
    public sealed class ManagePricePublicationButtonsCustomization : IViewModelCustomization<PriceViewModel>
    {
        public void Customize(PriceViewModel viewModel, ModelStateDictionary modelState)
        {
            var publishButton = viewModel.ViewConfig.FindCardToolbarItem("PublishPrice");
            var unpublishButton = viewModel.ViewConfig.FindCardToolbarItem("UnpublishPrice");

            if (viewModel.IsPublished)
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