using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates
{
    public class ManageAdvertisementTemplatePublicationButtonsCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementTemplateModel = (AdvertisementTemplateViewModel)viewModel;

            var publishButton = advertisementTemplateModel.ViewConfig.FindCardToolbarItem("PublishAdvertisementTemplate");
            var unpublishButton = advertisementTemplateModel.ViewConfig.FindCardToolbarItem("UnpublishAdvertisementTemplate");

            if (advertisementTemplateModel.IsPublished)
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