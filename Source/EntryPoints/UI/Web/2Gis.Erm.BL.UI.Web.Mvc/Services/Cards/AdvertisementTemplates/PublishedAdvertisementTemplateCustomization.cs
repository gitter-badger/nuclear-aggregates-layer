using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates
{
    public sealed class PublishedAdvertisementTemplateCustomization : IViewModelCustomization<AdvertisementTemplateViewModel>
    {
        public void Customize(AdvertisementTemplateViewModel viewModel, ModelStateDictionary modelState)
        {
            if (!viewModel.IsPublished)
            {
                return;
            }

            if (viewModel.MessageType != MessageType.None)
            {
                viewModel.SetInfo(BLResources.CanNotChangePublishedAdvertisementTemplate);
            }

            viewModel.ViewConfig.ReadOnly = true;
        }
    }
}