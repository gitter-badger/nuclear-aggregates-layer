using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates
{
    public sealed class ManageAdvertisementTemplatePublicationButtonsCustomization : IViewModelCustomization<AdvertisementTemplateViewModel>
    {
        public void Customize(AdvertisementTemplateViewModel viewModel, ModelStateDictionary modelState)
        {
            var publishButton = viewModel.ViewConfig.FindCardToolbarItem("PublishAdvertisementTemplate");
            var unpublishButton = viewModel.ViewConfig.FindCardToolbarItem("UnpublishAdvertisementTemplate");

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