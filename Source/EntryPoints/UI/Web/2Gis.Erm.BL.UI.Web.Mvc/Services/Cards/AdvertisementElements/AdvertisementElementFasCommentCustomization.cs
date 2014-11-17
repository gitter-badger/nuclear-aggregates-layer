using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public class AdvertisementElementFasCommentCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementElementModel = (AdvertisementElementViewModel)viewModel;
            
            if (advertisementElementModel.FasComment != null)
            {
                advertisementElementModel.FasComment.FasCommentDisplayTextItemsJson = FasCommentViewModelHelper.GetDisplayTextItemsJson();
            }
        }
    }
}