using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements
{
    public sealed class AdvertisementAccessCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            // FIXME {all, 18.11.2014}: техдолг: поле UserDoesntHaveRightsToEditFirm не требуется ни в dto ни во ViewModel.
            //                          можно вычислять права тут и по результатам выставлять ReadOnly
            var advertisementModel = (AdvertisementViewModel)viewModel;
            advertisementModel.ViewConfig.ReadOnly |= advertisementModel.UserDoesntHaveRightsToEditFirm;
        }
    }
}