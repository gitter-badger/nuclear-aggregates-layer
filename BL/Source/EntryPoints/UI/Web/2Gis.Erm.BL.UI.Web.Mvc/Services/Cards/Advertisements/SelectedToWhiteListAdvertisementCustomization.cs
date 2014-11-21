using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements
{
    public sealed class SelectedToWhiteListAdvertisementCustomization : IViewModelCustomization<AdvertisementViewModel>
    {
        public void Customize(AdvertisementViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsSelectedToWhiteList)
            {
                viewModel.SetInfo(BLResources.AdvertisementIsSelectedToWhiteList);
            }
        }
    }
}