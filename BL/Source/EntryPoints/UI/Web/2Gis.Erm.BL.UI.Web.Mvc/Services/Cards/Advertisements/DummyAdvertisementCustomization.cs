using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements
{
    public sealed class DummyAdvertisementCustomization : IViewModelCustomization<AdvertisementViewModel>
    {
        public void Customize(AdvertisementViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsDummy)
            {
                viewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}