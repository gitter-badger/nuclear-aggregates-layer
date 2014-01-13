using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class AdvertisementTemplateViewModelCustomizationService : IGenericViewModelCustomizationService<AdvertisementTemplate>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementTemplateModel = (AdvertisementTemplateViewModel)viewModel;

            if (advertisementTemplateModel.MessageType != MessageType.None && advertisementTemplateModel.IsPublished)
            {
                advertisementTemplateModel.SetInfo(BLResources.CanNotChangePublishedAdvertisementTemplate);
            }

            var publishButton = advertisementTemplateModel.ViewConfig.FindCardToolbarItem("PublishAdvertisementTemplate");

            if (!publishButton.Disabled && advertisementTemplateModel.IsPublished)
            {
                publishButton.Disabled = true;
            }

            var unpublishButton = advertisementTemplateModel.ViewConfig.FindCardToolbarItem("UnpublishAdvertisementTemplate");

            if (!unpublishButton.Disabled && !advertisementTemplateModel.IsPublished)
            {
                unpublishButton.Disabled = true;
            }

            if (advertisementTemplateModel.IsPublished)
            {
                advertisementTemplateModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}