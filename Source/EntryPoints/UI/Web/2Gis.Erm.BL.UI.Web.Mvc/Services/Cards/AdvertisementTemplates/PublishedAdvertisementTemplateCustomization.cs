using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates
{
    public sealed class PublishedAdvertisementTemplateCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementTemplateModel = (AdvertisementTemplateViewModel)viewModel;

            if (!advertisementTemplateModel.IsPublished)
            {
                return;
            }

            if (advertisementTemplateModel.MessageType != MessageType.None)
            {
                advertisementTemplateModel.SetInfo(BLResources.CanNotChangePublishedAdvertisementTemplate);
            }

            advertisementTemplateModel.ViewConfig.ReadOnly = true;
        }
    }
}