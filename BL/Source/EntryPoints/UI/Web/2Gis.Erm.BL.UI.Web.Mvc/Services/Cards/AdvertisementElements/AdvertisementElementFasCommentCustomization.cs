using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public sealed class AdvertisementElementFasCommentCustomization : IViewModelCustomization<AdvertisementElementViewModel>
    {
        public void Customize(AdvertisementElementViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.FasComment != null)
            {
                viewModel.FasComment.FasCommentDisplayTextItemsJson = FasCommentViewModelHelper.GetDisplayTextItemsJson();
            }
        }
    }
}